using Aqorn.Models.Data;
using Aqorn.Models.Spec;
using System.Diagnostics.CodeAnalysis;

namespace Aqorn.Models.DbModel;

internal class DbTable : ModelBase
{
    public bool IdentityInsert { get; }
    public string? Schema { get; }
    public string TableName { get; }
    public DbColumn[] Columns { get; }
    
    private readonly List<DbRow> _rows = [];
    public DbRow[] Rows => _rows.ToArray();

    public DbTable(ModelBase parent, TableSpec spec) : base(parent, spec.Name)
    {
        IdentityInsert = spec.IdentityInsert;
        Schema = spec.SchemaName;
        TableName = spec.TableName;
        Columns = spec.Fields.Select(f => new DbColumn(this, f)).ToArray();
    }

    public void AddData(TableModel table)
    {
        foreach (var row in table.Rows)
        {
            _rows.Add(new DbRow(this, row));
        }
    }

    public bool TryGetColumn(string name, [MaybeNullWhen(false)] out DbColumn column)
    {
        column = Columns.FirstOrDefault(f => f.Name == name);
        return column != null;
    }
}