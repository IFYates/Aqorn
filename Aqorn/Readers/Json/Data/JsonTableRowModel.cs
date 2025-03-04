using Aqorn.Models.Data;
using System.Text.Json;

namespace Aqorn.Readers.Json.Data;

internal class JsonTableRowModel : TableRowModel
{
    public JsonTableRowModel(IErrorLog errors, TableModel parent, JsonElement json)
        : base(parent)
    {
        var fields = new List<FieldModel>();
        var relations = new List<TableModel>();
        foreach (var fieldItem in json.EnumerateObject())
        {
            if (fieldItem.Name[0] == ':')
            {
                relations.Add(new JsonTableModel(errors.Step(fieldItem.Name[1..]), fieldItem.Name[1..], fieldItem.Value));
                continue;
            }

            fields.Add(new JsonFieldModel(errors.Step(fieldItem.Name), this, fieldItem.Name, fieldItem.Value));
        }
        Fields = fields.ToArray();
        Relationships = relations.ToArray();
    }
}