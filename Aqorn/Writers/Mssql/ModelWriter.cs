using Aqorn.Models.DbModel;
using System.Text;

namespace Aqorn.Writers.Mssql;

/// <summary>
/// Outputs the data model as MSSQL.
/// </summary>
internal class ModelWriter(/* options? */)
{
    public string GenerateSql(DbDataset dataset)
    {
        ArgumentNullException.ThrowIfNull(dataset);

        var sb = new StringBuilder();
        foreach (var table in dataset.Tables)
        {
            if (table.Rows.Length > 0)
            {
                TableWriter.GenerateSql(table, sb);
            }
        }

        return sb.ToString();
    }
}