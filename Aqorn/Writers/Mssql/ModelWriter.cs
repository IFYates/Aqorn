using Aqorn.Models.Data;
using Aqorn.Models.Spec;
using System.Text;

namespace Aqorn.Writers.Mssql;

/// <summary>
/// Outputs the data model as MSSQL.
/// </summary>
internal class ModelWriter(/* options? */)
{
    public string GenerateSql(IDataSchema data, ISchemaSpec spec)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(spec);

        var sb = new StringBuilder();
        foreach (var table in data.Tables)
        {
            TableWriter.GenerateSql(table, sb);
        }

        return sb.ToString();
    }
}
