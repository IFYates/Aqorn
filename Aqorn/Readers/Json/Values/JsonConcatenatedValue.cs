using Aqorn.Models.Values;
using Aqorn.Readers.Json.Values;
using System.Text.Json;

namespace Aqorn.Readers.Json;

internal sealed class JsonConcatenatedValue : ConcatenatedValue
{
    public JsonConcatenatedValue(IErrorLog errors, string fieldName, JsonElement json)
    {
        if (json.ValueKind != JsonValueKind.Array)
        {
            errors.Add($"Invalid concatenation type for '{fieldName}'.");
            return;
        }

        var values = new List<FieldValue>();
        foreach (var element in json.EnumerateArray())
        {
            if (element.ValueKind is JsonValueKind.String or JsonValueKind.Number or JsonValueKind.True or JsonValueKind.False or JsonValueKind.Null)
            {
                values.Add(new JsonFieldValue(element));
            }
            else
            {
                errors.Add("Concatenation only supports literal tokens.");
                return;
            }
        }
        Values = values.ToArray();
    }
}