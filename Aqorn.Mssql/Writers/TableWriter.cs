using Aqorn.Models;
using Aqorn.Models.DbModel;
using Aqorn.Models.Values;
using System.Text;

namespace Aqorn.Mssql.Writers;

public sealed class TableWriter(IOptions options)
{
    private readonly int _batchSize = options.InsertBatchSize;

    private static string getInsertHead(DbColumn[] columns)
    {
        var table = columns[0].Table;
        var head = new StringBuilder()
            .Append("INSERT INTO ").Append(tableName(table)).Append('(');
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
            sb.Append("SET IDENTITY_INSERT ").Append(tableName(table)).AppendLine(" ON")
                .AppendLine("GO");
        }

        var total = 0;
        foreach (var group in groups)
        {
            writeInsertGroupSql(sb, group.Key, group.Value, ref total);
        }

        if (table.IdentityInsert)
        {
            sb.Append("SET IDENTITY_INSERT ").Append(tableName(table)).AppendLine(" OFF")
                .AppendLine("GO");
        }
    }

    private void writeInsertGroupSql(StringBuilder sb, DbColumn[] columns, DbDataRow[] rows, ref int total)
    {
        var head = getInsertHead(columns);
        for (var i = 0; i < rows.Length; ++i)
        {
            if (i % _batchSize == 0)
            {
                if (i > 0)
                {
                    sb.Remove(sb.Length - 3, 1)
                        .AppendLine("GO -- " + (i + total));
                }
                sb.AppendLine(head);
            }

            sb.Append("    (");
            foreach (var field in columns.Join(rows[i].Fields, c => c.Name, f => f.Name, (c, f) => f))
            {
                writeFieldSql(sb, field.Value!);
                sb.Append(", ");
            }
            sb.Remove(sb.Length - 2, 2)
                .AppendLine("),");
        }
        sb.Remove(sb.Length - 3, 1)
            .AppendLine("GO -- " + (total += rows.Length));
    }

    private static void writeFieldSql(StringBuilder sb, IValue value)
    {
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
                    writeFieldSql(sb, f.Value!);
                    sb.Append(" AND ");
                }
                sb.Remove(sb.Length - 5, 5);
            }
            sb.Append(')');
            return;
        }
        switch (value.Type)
        {
            case FieldValue.ValueType.Boolean:
                sb.Append(((FieldValue)value).Value == bool.TrueString ? "1" : "0");
                break;
            case FieldValue.ValueType.Null:
                sb.Append("NULL");
                break;
            case FieldValue.ValueType.Number:
            case FieldValue.ValueType.Sql:
                sb.Append(value);
                break;
            default:
                sb.Append('\'').Append(value).Append('\'');
                break;
        }
    }

    private static string tableName(DbTable table)
        => tableName(table.SchemaName, table.TableName);
    private static string tableName(string? schemaName, string tableName)
        => (schemaName?.Length > 0 ? $"[{schemaName}]." : "") + $"[{tableName}]";
}