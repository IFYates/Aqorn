using Aqorn.Models;
using Aqorn.Models.Spec;
using System.Text.Json;

namespace Aqorn.Readers.Json.Spec;

internal class JsonFieldSpec : FieldSpec
{
    public JsonFieldSpec(IModel parent, string name, JsonElement json)
        : base(parent, name)
    {
        switch (json.ValueKind)
        {
            case JsonValueKind.True:
            case JsonValueKind.False:
            case JsonValueKind.Null:
            case JsonValueKind.Number:
                var fv = new JsonFieldValue(this, json);
                Value = fv;
                ValueType = new FieldTypeSpec(this, fv.Type);
                return;
            case JsonValueKind.String:
                ValueType = new FieldTypeSpec(this, json.GetString()!);
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