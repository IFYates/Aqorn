using Aqorn.Models.Data;
using Aqorn.Models.Values;
using Aqorn.Readers.Json.Data;
using System.Text.Json;

namespace Aqorn.Readers.Json.Values;

internal sealed class JsonSubqueryValue : SubqueryValue
{
    public JsonSubqueryValue(IErrorLog errors, JsonElement json)
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

        FieldsSpec = json.EnumerateObject()
            .Where(e => e.Name != "?")
            .Select(e => (IDataField)new JsonDataField(errors, null!, e.Name, e.Value))
            .ToArray();
    }
}