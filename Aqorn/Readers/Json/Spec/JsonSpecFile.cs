using Aqorn.Models;
using Aqorn.Models.Spec;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Aqorn.Readers.Json.Spec;

/// <summary>
/// Reads a JSON specification file and converts to spec models.
/// </summary>
internal class JsonSpecFile : ModelBase, ISchemaSpec
{
    public TableSpec[] Tables { get; }

    public JsonSpecFile(string jsonSpecFile)
        : base(null!, null!, new ModelValidator())
    {
        Tables = parse(jsonSpecFile);
    }

    private TableSpec[] parse(string file)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(file);

        var json = File.ReadAllText(file);

        JsonDocument doc;
        try
        {
            doc = JsonDocument.Parse(json, new JsonDocumentOptions { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip });
        }
        catch
        {
            Error("Invalid JSON file");
            return [];
        }
        if (doc.RootElement.ValueKind != JsonValueKind.Object)
        {
            Error("Bad file structure");
            return [];
        }

        return doc.RootElement.EnumerateObject()
            .Select(t => new JsonTableSpec(this, t.Name, t.Value)).ToArray();
    }

    public bool TryGetTable(string name, [MaybeNullWhen(false)] out TableSpec tableSpec)
    {
        tableSpec = Tables.FirstOrDefault(t => t.Name == name);
        return tableSpec != null;
    }

    public string[] Validate()
    {
        return Validator.Errors;
    }
}