using Aqorn.Models.Data;

namespace Aqorn.Models.DbModel;

internal class DbRow
    : ModelBase
{
    public string FieldKey
        => string.Join(',', Table.Columns.Where(c => c.IsRequired || Fields.Any(f => f.Column == c)).Select(c => c.Name));

    public DbRow(DbTable table, TableRowModel row)
        : base(table, null!)
    {
        Table = table;

        Fields = row.Fields.Select(f =>
        {
            if (!table.TryGetColumn(f.Name, out var c))
            {
                Error($"Unable to locate column '{f.Name}'.");
                return null!;
            }
            return new DbField(c, f);
        }).ToArray();
    }

    public DbTable Table { get; }
    public DbField[] Fields { get; }
}