using Aqorn.Models;
using Aqorn.Models.Data;
using Aqorn.Models.Spec;
using System.Text.Json;

namespace Aqorn.Readers.Json.Spec;

internal class JsonQueryValueSpec : QueryValueSpec
{
    public JsonQueryValueSpec(IModel parent, JsonElement json)
        : base(parent)
    {
        if (json.ValueKind != JsonValueKind.Object
            || !json.TryGetProperty("?", out var target)
            || target.ValueKind != JsonValueKind.String)
        {
            Error("Invalid query value.");
            return;
        }

        var targetStr = target.GetString()!;
        var period = targetStr.LastIndexOf('.');
        if (period < 0)
        {
            Error($"Invalid query target ('{targetStr}').");
            return;
        }

        TableName = targetStr[..period];
        FieldName = targetStr[(period + 1)..];

        Fields = json.EnumerateObject()
            .Where(e => e.Name != "?")
            .ToDictionary(e => e.Name, e => (FieldModel)new JsonDataField(this, e.Name, e.Value))
            .AsReadOnly();
    }
}