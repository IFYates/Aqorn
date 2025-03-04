using Aqorn.Models;
using Aqorn.Models.Spec;
using System.Text.Json;

namespace Aqorn.Readers.Json.Spec;

internal class JsonTableSpec : TableSpec
{
    public JsonTableSpec(IModel parent, string name, JsonElement json)
        : base(parent, name)
    {
        if (json.ValueKind != JsonValueKind.Object)
        {
            Error($"Table spec must be an object ('{Name}' gave {json.ValueKind}).");
            return;
        }

        TableName = Name;
        if (json.TryGetProperty("#", out var tableNameProp)
            && tableNameProp.ValueKind == JsonValueKind.String)
        {
            TableName = tableNameProp.GetString() ?? TableName;
        }

        var fields = new List<FieldSpec>();
        foreach (var fieldItem in json.EnumerateObject())
        {
            var value = fieldItem.Value;
            switch (fieldItem.Name)
            {
                case "#":
                    // Ignore
                    break;
                case ":identity":
                    IdentityInsert = value.ValueKind == JsonValueKind.True
                        || value.ValueKind == JsonValueKind.Number && value.GetDecimal() != 0
                        || value.ValueKind == JsonValueKind.String && value.GetString()?.Equals("true", StringComparison.InvariantCultureIgnoreCase) == true;
                    break;
                case ":relations":
                    Relationships = parseRelationships(fieldItem.Value);
                    break;
                default:
                    FieldSpec field = fieldItem.Name[0] == '@'
                        ? new JsonParameterSpec(this, fieldItem.Name, fieldItem.Value)
                        : new JsonFieldSpec(this, fieldItem.Name, fieldItem.Value);
                    fields.Add(field);
                    break;
            }
        }
        Fields = fields.ToArray();
    }

    private TableSpec[] parseRelationships(JsonElement json)
    {
        if (json.ValueKind != JsonValueKind.Object)
        {
            Error($"Table relationships spec must be an object ('{Name}' gave {json.ValueKind}).");
            return [];
        }

        return json.EnumerateObject()
            .Select(r => new JsonTableSpec(this, r.Name, r.Value)).ToArray();
    }
}