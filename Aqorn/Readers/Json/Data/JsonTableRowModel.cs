using Aqorn.Models;
using Aqorn.Models.Data;
using System.Text.Json;

namespace Aqorn.Readers.Json.Data;

internal class JsonTableRowModel
    : TableRowModel
{
    public JsonTableRowModel(IModel parent, string name, JsonElement json)
        : base(parent, name)
    {
        var fields = new List<FieldModel>();
        var relations = new List<TableModel>();
        foreach (var fieldItem in json.EnumerateObject())
        {
            if (fieldItem.Name[0] == ':')
            {
                relations.Add(new JsonTableModel(this, fieldItem.Name[1..], fieldItem.Value));
                continue;
            }

            fields.Add(new JsonFieldModel(this, fieldItem.Name, fieldItem.Value));
        }
        Fields = fields.ToArray();
        Relationships = relations.ToArray();
    }
}