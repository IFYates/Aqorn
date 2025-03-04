using Aqorn.Models.Data;
using Aqorn.Models.Values;
using Aqorn.Readers;
using System.Diagnostics.CodeAnalysis;

namespace Aqorn.Models.DbModel;

internal class DbRowData
{
    public DbRowData? ParentRow { get; }
    public DbRowField[] Fields { get; }

    public DbColumn[] ColumnList { get; }
    public string ColumnListKey => string.Join(',', ColumnList.Select(c => c.Name));

    public DbRowData(IErrorLog errors, DbTable table, DbRowData? drow, TableRowModel row)
    {
        ParentRow = drow;

        // Prepare fields
        var fields = row.Fields.ToDictionary(f => f.Name);
        var rows = new List<DbRowField>();
        foreach (var col in table.Columns)
        {
            if (fields.TryGetValue(col.Name, out var field))
            {
                rows.Add(new DbRowField(this, col, field.Value));
                fields.Remove(col.Name);
            }
            else if (col.IsRequired && col.DefaultValue == null)
            {
                errors.Add($"Missing required field in data ({col.Name}).");
            }
            else
            {
                rows.Add(new DbRowField(this, col, col.DefaultValue ?? FieldValue.Null));
            }
        }
        if (fields.Count > 0)
        {
            foreach (var field in fields.Values)
            {
                errors.Step(field.Name).Add("Unable to locate spec for field.");
            }
        }
        Fields = rows.Where(f => f.IncludeInInsert || f.IsParameter).ToArray();
        ColumnList = table.Columns.Where(c => Fields.Any(f => f.Column == c && !f.IsParameter)).ToArray();

        // Resolve in order
        int count = 1, lastCount = 0;
        while (count > 0 && lastCount != count)
        {
            lastCount = count;
            count = 0;
            foreach (var field in Fields)
            {
                if (!field.ResolveValue(errors))
                {
                    ++count;
                }
            }
        }
        if (count > 0)
        {
            errors.Add($"Circular dependency detected ({count}).");
        }
    }

    public bool TryGetField(string name, [MaybeNullWhen(false)] out DbRowField field)
    {
        field = Fields.FirstOrDefault(f => f.Name == name);
        return field != null;
    }
}