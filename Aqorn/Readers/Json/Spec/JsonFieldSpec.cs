using Aqorn.Models.Spec;
using System.Text.Json;

namespace Aqorn.Readers.Json.Spec;

internal class JsonFieldSpec : FieldSpec
{
    public JsonFieldSpec(IErrorLog errors, string name, JsonElement json)
        : base(name)
    {
        switch (json.ValueKind)
        {
            case JsonValueKind.True:
            case JsonValueKind.False:
            case JsonValueKind.Null:
            case JsonValueKind.Number:
                var fv = new JsonFieldValue(errors, json);
                Value = fv;
                ValueType = new FieldTypeSpec(fv.Type);
                return;
            case JsonValueKind.String:
                ValueType = new JsonFieldTypeSpec(errors, json.GetString()!);
                return;
            case JsonValueKind.Array:
                var cv = new JsonConcatenatedValue(errors, name, json);
                Value = cv.Values.Length == 1 ? cv.Values[0] : cv;
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