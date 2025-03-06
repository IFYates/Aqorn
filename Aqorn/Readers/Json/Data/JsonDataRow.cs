using Aqorn.Models.Data;
using System.Text.Json;

namespace Aqorn.Readers.Json.Data;

internal sealed class JsonDataRow : IDataRow
{
    public IDataTable Table { get; }
    public IDataField[] Fields { get; }
    public IDataTable[] Relationships { get; }

    public JsonDataRow(IErrorLog errors, IDataTable table, JsonElement json)
    {
        Table = table;

        var fields = new Dictionary<string, IDataField>();
        foreach (var param in table.Schema.Parameters)
        {
            fields[param.Name] = param;
        }

        var relations = new List<IDataTable>();
        foreach (var fieldItem in json.EnumerateObject())
        {
            if (fieldItem.Name[0] == ':')
            {
                relations.Add(new JsonDataTable(errors, fieldItem.Name[1..], fieldItem.Value, table.Schema));
            }
            else
            {
                fields[fieldItem.Name] = new JsonDataField(errors, fieldItem.Name, fieldItem.Value);
            }
        }

        Fields = fields.Values.ToArray();
        Relationships = relations.ToArray();
    }
}