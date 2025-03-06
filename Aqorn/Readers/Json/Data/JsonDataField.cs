using Aqorn.Models.Data;
using Aqorn.Models.Values;
using Aqorn.Readers.Json.Values;
using System.Text.Json;

namespace Aqorn.Readers.Json.Data;

internal sealed class JsonDataField : IDataField
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
                Value = FieldValue.Boolean(json.ValueKind == JsonValueKind.True);
                return;
            case JsonValueKind.Number:
                Value = FieldValue.Number(json.GetRawText());
                return;
            case JsonValueKind.String:
                Value = FieldValue.String(json.GetString());
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