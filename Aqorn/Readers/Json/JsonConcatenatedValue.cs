using Aqorn.Models;
using System.Text.Json;

namespace Aqorn.Readers.Json;

internal class JsonConcatenatedValue : ConcatenatedValue
{
    public JsonConcatenatedValue(IModel field, JsonElement json)
        : base(field)
    {
        if (json.ValueKind != JsonValueKind.Array)
        {
            Error($"Invalid concatenation type for '{field.Name}'.");
            return;
        }

        var values = new List<FieldValue>();
        foreach (var element in json.EnumerateArray())
        {
            if (element.ValueKind is JsonValueKind.String or JsonValueKind.Number or JsonValueKind.True or JsonValueKind.False or JsonValueKind.Null)
            {
                values.Add(new JsonFieldValue(this, element));
            }
            else
            {
                Error("Concatenation only supports literal tokens.");
                return;
            }
        }
        Values = values.ToArray();
    }
}
