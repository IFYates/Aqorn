using Aqorn.Models;
using Aqorn.Models.Spec;
using Aqorn.Models.Values;
using Aqorn.Readers.Json.Values;
using System.Text.Json;

namespace Aqorn.Readers.Json.Spec;

public sealed class JsonColumnSpec : IColumnSpec
{
    public string Name { get; }
    public IFieldTypeSpec? ValueType { get; }
    public IValue? DefaultValue { get; }

    public JsonColumnSpec(IErrorLog errors, string name, JsonElement json)
    {
        Name = name;

        errors = errors.Step(name);
        switch (json.ValueKind)
        {
            case JsonValueKind.Null:
                DefaultValue = FieldValue.Null;
                break;
            case JsonValueKind.True:
            case JsonValueKind.False:
                DefaultValue = FieldValue.Boolean(json.ValueKind == JsonValueKind.True);
                break;
            case JsonValueKind.Number:
                DefaultValue = FieldValue.Number(json.GetRawText());
                break;

            case JsonValueKind.String:
                var str = json.GetString()!;
                if (JsonFieldTypeSpec.TypeFormat().IsMatch(str))
                {
                    ValueType = new JsonFieldTypeSpec(errors, str);
                }
                else
                {
                    DefaultValue = new JsonFieldValue(json);
                }
                break;
            case JsonValueKind.Array:
                var cv = new JsonConcatenatedValue(errors, name, json);
                DefaultValue = cv.Values.Length == 1 ? cv.Values[0] : cv;
                break;
            case JsonValueKind.Object:
                if (json.TryGetProperty("?", out _))
                {
                    DefaultValue = new JsonSubqueryValue(errors, json);
                }
                break;
        }

        if ((ValueType?.Type ?? DefaultValue?.Type) is null or FieldValue.ValueType.Unknown)
        {
            errors.Add("Invalid field spec ({json.ValueKind}).");
        }
    }
}