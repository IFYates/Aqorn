using Aqorn.Models;
using Aqorn.Models.Data;
using Aqorn.Readers.Json.Spec;
using System.Text.Json;

namespace Aqorn.Readers.Json;

internal class JsonDataField : FieldModel
{
    public JsonDataField(IModel parent, string name, JsonElement json)
        : base(parent, name)
    {
        switch (json.ValueKind)
        {
            case JsonValueKind.String:
            case JsonValueKind.True:
            case JsonValueKind.False:
            case JsonValueKind.Null:
            case JsonValueKind.Number:
                Value = new JsonFieldValue(this, json);
                return;
            case JsonValueKind.Array:
                Value = new JsonConcatenatedValue(this, json);
                return;
            case JsonValueKind.Object:
                if (json.TryGetProperty("?", out _))
                {
                    Value = new JsonQueryValueSpec(this, json);
                    return;
                }
                break;
        }
        Error($"Invalid field spec ({json.ValueKind}).");
    }
}
