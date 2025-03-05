using Aqorn.Models.DbModel;
using Aqorn.Models.Values;
using System.Text;

namespace Aqorn.Writers.Mssql;

internal sealed class TableWriter(/* options? */)
{
    private const int BATCH = 100; // TODO: option?

    private static string getInsertHead(DbColumn[] columns)
    {
        var table = columns[0].Table;
        var head = new StringBuilder()
            .Append("INSERT INTO ").Append(tableName(table.SchemaName, table.TableName)).Append('(');
        foreach (var col in columns)
        {
            head.Append('[').Append(col.Name).Append("], ");
        }
        head.Remove(head.Length - 2, 2)
            .Append(") VALUES");
        return head.ToString();
    }

    public void GenerateSql(DbTable table, StringBuilder sb)
    {
        var groups = table.Rows.Where(r => r.ColumnList.Length > 0)
            .GroupBy(r => r.ColumnListKey)
            .ToDictionary(g => g.First().ColumnList, g => g.ToArray());
        if (groups.Count == 0)
        {
            return;
        }

        if (table.IdentityInsert)
        {
            sb.Append("SET IDENTITY_INSERT ").Append(table.Name).AppendLine(" ON")
                .AppendLine("GO");
        }

        var total = 0;
        foreach (var group in groups)
        {
            writeInsertGroupSql(sb, group.Key, group.Value, ref total);
        }

        if (table.IdentityInsert)
        {
            sb.Append("SET IDENTITY_INSERT ").Append(table.Name).AppendLine(" OFF")
                .AppendLine("GO");
        }
    }

    private static void writeInsertGroupSql(StringBuilder sb, DbColumn[] columns, DbDataRow[] rows, ref int total)
    {
        var head = getInsertHead(columns);
        var count = 0;
        foreach (var row in rows)
        {
            if (count++ % BATCH == 0)
            {
                if (count > 1)
                {
                    sb.Remove(sb.Length - 1, 1)
                        .AppendLine("GO -- " + (count + total));
                }
                sb.AppendLine(head);
            }

            sb.Append("    (");
            foreach (var col in columns)
            {
                if (row.TryGetField(col.Name, out var field))
                {
                    writeFieldSql(sb, field.Value!);
                    sb.Append(", ");
                }
                else
                {
                    sb.Append("NULL, ");
                }
            }
            sb.Remove(sb.Length - 2, 2)
                .AppendLine("),");
        }
        total += count;
        sb.Remove(sb.Length - 3, 1)
            .AppendLine("GO -- " + total);
    }

    private static void writeFieldSql(StringBuilder sb, IValue? value)
    {
        if (value == null)
        {
            sb.Append("NULL");
            return;
        }
        if (value is SubqueryValue qv)
        {
            sb.Append("(SELECT TOP 1 [").Append(qv.FieldName).Append("] FROM ")
                .Append(tableName(qv.SchemaName, qv.TableName));
            if (qv.Fields.Length > 0)
            {
                sb.Append(" WHERE ");
                foreach (var f in qv.Fields)
                {
                    sb.Append('[').Append(f.Name).Append("] = ");
                    writeFieldSql(sb, f.Value ?? FieldValue.Null);
                    sb.Append(" AND ");
                }
                sb.Remove(sb.Length - 5, 5);
            }
            sb.Append(')');
            return;
        }
        switch (value.Type)
        {
            case FieldValue.ValueType.Null:
                sb.Append("NULL");
                break;
            case FieldValue.ValueType.Boolean:
            case FieldValue.ValueType.Number:
            case FieldValue.ValueType.Sql:
                sb.Append(value);
                break;
            default:
                sb.Append('\'').Append(value).Append('\'');
                break;
        }
    }

    private static string tableName(string? schemaName, string tableName)
        => (schemaName?.Length > 0 ? $"[{schemaName}]." : "") + $"[{tableName}]";
}