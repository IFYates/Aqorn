using Aqorn.Models.Data;
using Aqorn.Models.Spec;
using System.Text.Json;

namespace Aqorn.Readers.Json.Spec;

internal class JsonQueryValueSpec : QueryValueSpec
{
    public JsonQueryValueSpec(IErrorLog errors, JsonElement json)
    {
        if (json.ValueKind != JsonValueKind.Object
            || !json.TryGetProperty("?", out var target)
            || target.ValueKind != JsonValueKind.String)
        {
            errors.Add("Invalid query value.");
            return;
        }

        var targetStr = target.GetString()!;
        var period = targetStr.LastIndexOf('.');
        if (period < 0)
        {
            errors.Add($"Invalid query target ('{targetStr}').");
            return;
        }
        FieldName = targetStr[(period + 1)..];

        targetStr = targetStr[..period];
        period = targetStr.IndexOf('.');
        if (period >= 0)
        {
            SchemaName = targetStr[..period];
            TableName = targetStr[(period + 1)..];
        }
        else
        {
            SchemaName = null;
            TableName = targetStr;
        }

        Fields = json.EnumerateObject()
            .Where(e => e.Name != "?")
            .Select(e => (FieldModel)new JsonDataField(errors, e.Name, e.Value))
            .ToArray();
    }
}