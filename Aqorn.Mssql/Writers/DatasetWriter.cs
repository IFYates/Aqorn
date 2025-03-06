using Aqorn.Models.DbModel;
using System.Text;

namespace Aqorn.Mssql.Writers;

/// <summary>
/// Outputs the data model as MSSQL.
/// </summary>
public sealed class DatasetWriter(/* options */)
{
    private readonly TableWriter _tableWriter = new(/* options */);

    public string GenerateSql(DbDataset dataset)
    {
        ArgumentNullException.ThrowIfNull(dataset);

        var sb = new StringBuilder();

        // TODO: handle dependency chain
        var queue = new Queue<DbTable>(dataset.Tables);
        while (queue.TryDequeue(out var table))
        {
            if (table.Rows.Length > 0)
            {
                sb.AppendLine($"-- {table.Name} ({table.Rows.Length})");
                _tableWriter.GenerateSql(table, sb);
                sb.AppendLine();
            }

            foreach (var rel in table.Relationships)
            {
                queue.Enqueue(rel);
            }
        }

        return sb.ToString();
    }
}