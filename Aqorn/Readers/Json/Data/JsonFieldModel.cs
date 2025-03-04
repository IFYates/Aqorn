using Aqorn.Models.Data;
using System.Text.Json;

namespace Aqorn.Readers.Json.Data;

internal class JsonFieldModel : FieldModel
{
    public JsonFieldModel(IErrorLog errors, TableRowModel parent, string name, JsonElement json)
        : base(parent, name)
    {
        switch (json.ValueKind)
        {
            case JsonValueKind.True:
            case JsonValueKind.False:
            case JsonValueKind.Null:
            case JsonValueKind.Number:
            case JsonValueKind.String:
                Value = new JsonFieldValue(errors, json, true);
                return;
            case JsonValueKind.Array:
                var cv = new JsonConcatenatedValue(errors, name, json);
                Value = cv.Values.Length == 1 ? cv.Values[0] : cv;
                return;
            //case JsonValueKind.Object:
            //    if (json.TryGetProperty("?", out _))
            //    {
            //        Value = new JsonQueryValueSpec(this, json);
            //        return;
            //    }
            //    break;
        }
        errors.Add($"Invalid field definition ({json.ValueKind}).");
    }
}