using Aqorn.Models.Data;
using Aqorn.Models.Spec;
using Aqorn.Readers;
using System.Diagnostics.CodeAnalysis;

namespace Aqorn.Models.DbModel;

internal class DbTable
{
    public DbTable? Parent { get; }
    public DbDataset Dataset { get; }

    public TableSpec Spec { get; }
    public string Name => Spec.Name;
    public bool IdentityInsert => Spec.IdentityInsert;
    public string? SchemaName => Spec.SchemaName;
    public string TableName => Spec.TableName;
    public DbColumn[] Columns { get; }
    public DbTable[] Relationships { get; }
    
    private readonly List<DbRowData> _rows = [];
    public DbRowData[] Rows => _rows.ToArray();

    public DbTable(IErrorLog errors, DbTable parent, TableSpec spec)
        : this(errors, parent.Dataset, spec)
    {
        Parent = parent;
    }
    public DbTable(IErrorLog errors, DbDataset dataset, TableSpec spec)
    {
        errors = errors.Step(spec.Name);
        Dataset = dataset;
        Spec = spec;
        Columns = spec.Fields.Select(f => new DbColumn(this, f)).ToArray();
        Relationships = spec.Relationships.Select(r => new DbTable(errors, this, r)).ToArray();
    }

    public void AddData(IErrorLog errors, TableModel table)
        => addData(errors, table, null);
    private void addData(IErrorLog errors, TableModel table, DbRowData? parent)
    {
        foreach (var row in table.Rows)
        {
            var drow = new DbRowData(errors, this, parent, row);
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
                    errors = errors.Step(relData.Name);
                    rel.addData(errors, relData, drow);
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