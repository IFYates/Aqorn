using Aqorn.Models;
using Aqorn.Models.Data;
using System.Text.Json;

namespace Aqorn.Readers.Json.Data;

internal class JsonFieldModel : FieldModel
{
    public JsonFieldModel(IModel parent, string name, JsonElement json)
        : base(parent, name)
    {
        switch (json.ValueKind)
        {
            case JsonValueKind.True:
            case JsonValueKind.False:
            case JsonValueKind.Null:
            case JsonValueKind.Number:
            case JsonValueKind.String:
                var value = new JsonFieldValue(this, json, true);
                Value = value;
                return;
            case JsonValueKind.Array:
                Value = new JsonConcatenatedValue(this, json);
                return;
            //case JsonValueKind.Object:
            //    if (json.TryGetProperty("?", out _))
            //    {
            //        Value = new JsonQueryValueSpec(this, json);
            //        return;
            //    }
            //    break;
        }
        Error($"Invalid field definition ({json.ValueKind}).");
    }
}