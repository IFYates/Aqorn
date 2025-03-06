using Aqorn.Models;
using Aqorn.Models.Spec;
using System.Text.Json;

namespace Aqorn.Readers.Json.Spec;

public sealed class JsonTableSpec : ITableSpec
{
    public ITableSpec? Parent { get; }
    public string Name { get; }
    public string? SchemaName { get; }
    public string TableName { get; }

    public bool IdentityInsert { get; }
    public IColumnSpec[] Columns { get; }
    public ITableSpec[] Relationships { get; }

    public JsonTableSpec(IErrorLog errors, string name, JsonElement json, ITableSpec? parent)
    {
        Parent = parent;
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

        var properties = json.EnumerateObject().ToDictionary(p => p.Name, p => p.Value);

        // Split schema and table name
        if (properties.Remove("#", out var tableNameProp)
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

        if (properties.Remove(":identity", out var identityProp))
        {
            IdentityInsert = identityProp.ValueKind == JsonValueKind.True
                || (identityProp.ValueKind == JsonValueKind.Number && identityProp.GetDecimal() != 0)
                || (identityProp.ValueKind == JsonValueKind.String && identityProp.GetString()?.Equals("true", StringComparison.InvariantCultureIgnoreCase) == true);
        }

        var hasRelations = properties.Remove(":relations", out var relationProp);

        var columns = new Dictionary<string, IColumnSpec>();
        foreach (var prop in properties.Where(p => p.Key[0] != ':'))
        {
            IColumnSpec column = prop.Key[0] == '@'
                ? new JsonParameterSpec(errors, prop.Key, prop.Value)
                : new JsonColumnSpec(errors, prop.Key, prop.Value);
            columns[column.Name] = column;
        }
        Columns = columns.Values.ToArray();

        if (hasRelations)
        {
            if (relationProp.ValueKind == JsonValueKind.Object)
            {
                Relationships = relationProp.EnumerateObject()
                    .Select(r => new JsonTableSpec(errors, r.Name, r.Value, this)).ToArray();
            }
            else
            {
                errors.Add($"Table relationships spec must be an object (was {json.ValueKind}).");
            }
        }
    }
}