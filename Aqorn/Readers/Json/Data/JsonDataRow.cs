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

        var fields = new List<IDataField>();
        var relations = new List<IDataTable>();
        foreach (var fieldItem in json.EnumerateObject())
        {
            if (fieldItem.Name[0] == ':')
            {
                relations.Add(new JsonDataTable(errors, fieldItem.Name[1..], fieldItem.Value));
                continue;
            }

            fields.Add(new JsonDataField(errors, this, fieldItem.Name, fieldItem.Value));
        }
        Fields = fields.ToArray();
        Relationships = relations.ToArray();
    }
}