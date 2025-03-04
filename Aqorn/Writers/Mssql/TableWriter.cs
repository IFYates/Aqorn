using Aqorn.Models;
using Aqorn.Models.Data;
using Aqorn.Models.Spec;
using System.Text;

namespace Aqorn.Writers.Mssql;

internal class TableWriter
{
    private const int BATCH = 100; // TODO: option?

    private static string getInsertHead(TableSpec spec, out FieldSpec[] fieldSpecs)
    {
        var head = new StringBuilder()
            .AppendFormat("INSERT INTO [{0}].[{1}] (", spec.SchemaName, spec.TableName);
        fieldSpecs = spec.Fields.Where(f => f.Name[0] != '@').ToArray();
        foreach (var fieldSpec in fieldSpecs)
        {
            head.AppendFormat("[{0}], ", fieldSpec.Name);
        }
        head.Remove(head.Length - 2, 2)
            .Append(") VALUES");
        return head.ToString();
    }

    public static void GenerateSql(TableModel table, StringBuilder sb)
    {
        var head = getInsertHead(table.Spec!, out var fieldSpecs);
        var count = 0;
        foreach (var row in table.Rows)
        {
            if (count++ % BATCH == 0)
            {
                if (count > 1)
                {
                    sb.Remove(sb.Length - 1, 1)
                        .AppendLine("GO -- " + count);
                }
                sb.AppendLine(head);
            }

            // TODO: group by fields used
            sb.Append("    (");
            foreach (var fieldSpec in fieldSpecs)
            {
                if (row.TryGetField(fieldSpec.Name, out var field))
                {
                    writeFieldSql(sb, field.Value, fieldSpec, row);
                }
                else if (fieldSpec.Value != null)
                {
                    writeFieldSql(sb, fieldSpec.Value, fieldSpec, row);
                }
                else
                {
                    sb.Append("NULL, ");
                }
            }
            sb.Remove(sb.Length - 2, 2)
                .AppendLine("),");
        }

        sb.AppendLine("GO -- " + count);
    }

    private static void writeFieldSql(StringBuilder sb, ValueBase fieldValue, FieldSpec spec, TableRowModel row)
    {
        string? value;
        if (fieldValue is ConcatenatedValue cv)
        {
            value = string.Join("", cv.Values.Select(v => getFieldValue(v, row)));
            sb.Append('\'').Append(value.Replace("'", "''")).Append("', ");
            return;
        }
        if (fieldValue is FieldValue fv)
        {
            value = getFieldValue(fv, row);
        }
        else
        {
            throw new NotImplementedException();
        }

        // TODO: resolve self/parent earlier
        switch (spec.ValueType!.Type)
        {
            case FieldValue.ValueType.Null:
                sb.Append("NULL");
                break;
            case FieldValue.ValueType.Number:
            case FieldValue.ValueType.Boolean:
                sb.Append(value);
                break;
            default:
                sb.Append('\'').Append(value.Replace("'", "''")).Append('\'');
                break;
        }
        sb.Append(", ");
    }

    private static string getFieldValue(ValueBase value, TableRowModel row)
    {
        if (value is ConcatenatedValue cv)
        {
            // TODO: drop it by resolving TableRowModel concats earlier
            return string.Join("", cv.Values.Select(v => getFieldValue(v, row)));
        }
        if (value is not FieldValue fv)
        {
            throw new NotImplementedException();
        }

        switch (fv.Type)
        {
            case FieldValue.ValueType.Null:
                return "NULL";
            case FieldValue.ValueType.Parameter or FieldValue.ValueType.Self:
                return row.TryGetField(fv.Value, out var selfField) == true
                    ? getFieldValue(selfField.Value, row) : "NULL";
            case FieldValue.ValueType.Parent:
                var parentRow = row.Parent?.Parent as TableRowModel;
                return parentRow?.TryGetField(fv.Value, out var parField) == true
                    ? getFieldValue(parField.Value, row) : "NULL";
        }
        return fv.Value;
    }
}