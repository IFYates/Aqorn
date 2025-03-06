using Aqorn.Models.Data;
using Aqorn.Models.Spec;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Aqorn.Models.DbModel;

public sealed class DbTable
{
    public DbTable? Parent { get; }
    public DbDataset Dataset { get; }

    public ITableSpec Spec { get; }
    public string Name => Spec.Name;
    public bool IdentityInsert => Spec.IdentityInsert;
    public string? SchemaName => Spec.SchemaName;
    public string TableName => Spec.TableName;
    public DbColumn[] Columns { get; }
    public DbColumn[] Parameters => Columns.Where(c => c.Name[0] == '@').ToArray();
    public DbTable[] Relationships { get; }

    private readonly List<DbDataRow> _rows = [];
    public DbDataRow[] Rows => _rows.ToArray();

    public DbTable(IErrorLog errors, DbTable parent, ITableSpec spec)
        : this(errors, parent.Dataset, parent, spec)
    {
    }
    public DbTable(IErrorLog errors, DbDataset dataset, ITableSpec spec)
        : this(errors, dataset, null, spec)
    {
    }
    private DbTable(IErrorLog errors, DbDataset dataset, DbTable? parent, ITableSpec spec)
    {
        Parent = parent;
        Dataset = dataset;
        Spec = spec;

        var columns = spec.Columns.Select(f => new DbColumn(this, f)).ToArray();
        var parameters = parent?.Parameters.Select(p => new DbColumn(this, p.Spec)).ToArray()
            ?? dataset?.Parameters.Select(p => new DbColumn(this, p)).ToArray() ?? [];
        Columns = columns.UnionBy(parameters, a => a.Name).ToArray();

        errors = errors.Step(spec.Name);
        Relationships = spec.Relationships.Select(r => new DbTable(errors, this, r)).ToArray();
    }

    public void AddData(IErrorLog errors, IDataTable table)
        => addData(errors, table, null);
    private void addData(IErrorLog errors, IDataTable table, DbDataRow? parent)
    {
        foreach (var row in table.Rows)
        {
            var drow = new DbDataRow(errors, this, parent, row);
            _rows.Add(drow);

            foreach (var relData in row.Relationships)
            {
                var rel = Relationships.FirstOrDefault(r => r.Name == relData.Name);
                if (rel == null)
                {
                    errors.Add($"Reference to undefined child table ({relData.Name}).");
                }
                else
                {
                    rel.addData(errors.Step(relData.Name), relData, drow);
                }
            }
        }
    }

    public bool TryGetColumn(string name, [MaybeNullWhen(false)] out DbColumn column)
    {
        column = Columns.FirstOrDefault(f => f.Name == name);
        return column != null;
    }
}