using Aqorn.Models;
using Aqorn.Models.Data;
using Aqorn.Models.Values;
using Aqorn.Readers.Json.Values;
using System.Text.Json;

namespace Aqorn.Readers.Json.Data;

public sealed class JsonDataField : IDataField
{
    public string Name { get; }
    public IValue Value { get; }

    public JsonDataField(IErrorLog errors, string name, JsonElement json)
    {
        Name = name;

        switch (json.ValueKind)
        {
            case JsonValueKind.Null:
                Value = FieldValue.Null;
                return;
            case JsonValueKind.True:
            case JsonValueKind.False:
            case JsonValueKind.Number:
            case JsonValueKind.String:
                Value = new JsonFieldValue(json);
                return;

            case JsonValueKind.Array:
                var cv = new JsonConcatenatedValue(errors, name, json);
                Value = cv.Values.Length == 1 ? cv.Values[0] : cv;
                return;
            case JsonValueKind.Object:
                if (json.TryGetProperty("?", out _))
                {
                    Value = new JsonSubqueryValue(errors, json);
                    return;
                }
                break;
        }

        errors.Add($"Invalid field definition ({json.ValueKind}).");
        Value = FieldValue.Null;
    }
}