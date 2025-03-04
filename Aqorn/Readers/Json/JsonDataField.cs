using Aqorn.Models.Data;
using Aqorn.Models.Values;
using Aqorn.Readers.Json.Spec;
using System.Text.Json;

namespace Aqorn.Readers.Json;

internal class JsonDataField : FieldModel
{
    public JsonDataField(IErrorLog errors, string name, JsonElement json)
        : base((IValue)null!, name) // TODO
    {
        switch (json.ValueKind)
        {
            case JsonValueKind.String:
            case JsonValueKind.True:
            case JsonValueKind.False:
            case JsonValueKind.Null:
            case JsonValueKind.Number:
                Value = new JsonFieldValue(errors, json);
                return;
            case JsonValueKind.Array:
                Value = new JsonConcatenatedValue(errors, name, json);
                return;
            case JsonValueKind.Object:
                if (json.TryGetProperty("?", out _))
                {
                    Value = new JsonQueryValueSpec(errors, json);
                    return;
                }
                break;
        }
        errors.Add($"Invalid field spec ({json.ValueKind}).");
    }
}