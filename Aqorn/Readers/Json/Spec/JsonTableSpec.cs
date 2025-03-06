using Aqorn.Models.Spec;
using System.Text.Json;

namespace Aqorn.Readers.Json.Spec;

internal sealed class JsonTableSpec : ITableSpec
{
    public ITableSpec? Parent { get; }
    public ISpecSchema Schema { get; }
    public string Name { get; }
    public string? SchemaName { get; }
    public string TableName { get; }

    public bool IdentityInsert { get; }
    public IEnumerable<IColumnSpec> Columns { get; }
    public ITableSpec[] Relationships { get; }

    public JsonTableSpec(IErrorLog errors, string name, JsonElement json, ITableSpec parent)
        : this(errors, name, json, parent.Schema)
    {
        Parent = parent;
    }
    public JsonTableSpec(IErrorLog errors, string name, JsonElement json, ISpecSchema schema)
    {
        Schema = schema;
        Name = name;
        Relationships = [];

        errors = errors.Step(name);
        if (json.ValueKind != JsonValueKind.Object)
        {
            TableName = Name;
            Columns = [];
            errors.Add($"Table spec must be an object ('{name}' gave {json.ValueKind}).");
            return;
        }

        // Split schema and table name
        if (json.TryGetProperty("#", out var tableNameProp)
            && tableNameProp.ValueKind == JsonValueKind.String)
        {
            name = tableNameProp.GetString() ?? name;
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

        var fields = new List<IColumnSpec>();
        Columns = fields.AsReadOnly();
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
                            .Select(r => new JsonTableSpec(errors, r.Name, r.Value, this)).ToArray();
                    }
                    else
                    {
                        errors.Add($"Table relationships spec must be an object (was {json.ValueKind}).");
                    }
                    break;

                default:
                    IColumnSpec field = fieldItem.Name[0] == '@'
                        ? new JsonParameterSpec(errors, fieldItem.Name, fieldItem.Value)
                        : new JsonColumnSpec(errors, this, fieldItem.Name, fieldItem.Value);
                    fields.Add(field);
                    break;
            }
        }
        foreach (var parameter in schema.Parameters)
        {
            if (!fields.Any(f => f.Name == parameter.Name))
            {
                fields.Add(parameter);
            }
        }
    }
}