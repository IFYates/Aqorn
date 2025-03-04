using Aqorn.Models.Spec;
using System.Text.Json;

namespace Aqorn.Readers.Json.Spec;

internal class JsonTableSpec : TableSpec
{
    public JsonTableSpec(IErrorLog errors, string name, JsonElement json)
        : base(name)
    {
        if (json.ValueKind != JsonValueKind.Object)
        {
            errors.Add($"Table spec must be an object ('{Name}' gave {json.ValueKind}).");
            return;
        }

        // Split schema and table name
        if (json.TryGetProperty("#", out var tableNameProp)
            && tableNameProp.ValueKind == JsonValueKind.String)
        {
            name = tableNameProp.GetString() ?? TableName;
        }
        var period = name.IndexOf('.');
        if (period >= 0)
        {
            SchemaName = name[..period];
            TableName = name[(period + 1)..];
        }
        else
        {
            SchemaName = null;
            TableName = name;
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
                        || (value.ValueKind == JsonValueKind.Number && value.GetDecimal() != 0)
                        || (value.ValueKind == JsonValueKind.String && value.GetString()?.Equals("true", StringComparison.InvariantCultureIgnoreCase) == true);
                    break;
                case ":relations":
                    if (fieldItem.Value.ValueKind == JsonValueKind.Object)
                    {
                        Relationships = fieldItem.Value.EnumerateObject()
                            .Select(r => new JsonTableSpec(errors.Step(r.Name), r.Name, r.Value)).ToArray();
                    }
                    else
                    {
                        errors.Add($"Table relationships spec must be an object (was {json.ValueKind}).");
                    }
                    break;

                default:
                    FieldSpec field = fieldItem.Name[0] == '@'
                        ? new JsonParameterSpec(errors.Step(fieldItem.Name), fieldItem.Name, fieldItem.Value)
                        : new JsonFieldSpec(errors.Step(fieldItem.Name), fieldItem.Name, fieldItem.Value);
                    fields.Add(field);
                    break;
            }
        }
        Fields = fields.ToArray();
    }
}