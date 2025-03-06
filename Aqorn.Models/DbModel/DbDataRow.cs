using Aqorn.Models.Data;
using Aqorn.Models.Values;
using System.Diagnostics.CodeAnalysis;

namespace Aqorn.Models.DbModel;

public sealed class DbDataRow
{
    public DbTable Table { get; }
    public DbDataRow? ParentRow { get; }
    public DbRowField[] Fields { get; }
    public DbRowField[] Parameters => Fields.Where(f => f.Name[0] == '@').ToArray();

    public DbColumn[] ColumnList { get; }
    public string ColumnListKey => string.Join(',', ColumnList.Select(c => c.Name));

    public DbDataRow(IErrorLog errors, DbTable table, DbDataRow? parent, IDataRow row)
    {
        Table = table;
        ParentRow = parent;

        var parameters = ParentRow?.Parameters.Select(p => new ConstField(p.Name, p.Value!)).ToArray()
            ?? row.Table.Schema?.Parameters ?? [];
        var dataFields = row.Fields.UnionBy(parameters, f => f.Name)
            .ToDictionary(f => f.Name);

        // Prepare fields
        var rowFields = new List<DbRowField>();
        foreach (var col in table.Columns)
        {
            if (dataFields.Remove(col.Name, out var field))
            {
                rowFields.Add(new DbRowField(this, col, field.Value));
            }
            else if (col.IsRequired && col.DefaultValue == null)
            {
                errors.Add($"Missing required field in data ({col.Name}).");
            }
            else
            {
                rowFields.Add(new DbRowField(this, col, col.DefaultValue ?? FieldValue.Null));
            }
        }
        if (dataFields.Count > 0)
        {
            foreach (var field in dataFields.Values.Where(f => f.Name[0] != '@'))
            {
                errors.Add($"Unable to locate spec for field ({field.Name}).");
            }
        }

        Fields = rowFields.Where(f => f.IncludeInInsert || f.IsParameter).ToArray();
        ColumnList = table.Columns.Where(c => Fields.Any(f => f.Column == c && !f.IsParameter)).ToArray();

        // Resolve in order
        int count = -1, lastCount = 0;
        while (count != 0 && lastCount != count)
        {
            lastCount = count;
            count = Fields.Count(f => !f.TryApplyValue(errors));
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