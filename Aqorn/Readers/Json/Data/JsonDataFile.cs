using Aqorn.Models;
using Aqorn.Models.Data;
using Aqorn.Models.Spec;
using System.Text.Json;

namespace Aqorn.Readers.Json.Data;

/// <summary>
/// Reads a JSON data file and converts to models.
/// </summary>
internal class JsonDataFile : ModelBase, IDataSchema
{
    public TableModel[] Tables { get; }

    public JsonDataFile(string file)
        : base(null!, null!, new ModelValidator())
    {
        Tables = parse(file);
    }

    private TableModel[] parse(string file)
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
            Error("Invalid JSON file.");
            return [];
        }
        if (doc.RootElement.ValueKind != JsonValueKind.Object)
        {
            Error("Bad file structure.");
            return [];
        }

        return doc.RootElement.EnumerateObject()
            .Select(t => new JsonTableModel(this, t.Name, t.Value)).ToArray();
    }

    public string[] Validate(ISchemaSpec spec)
    {
        foreach (var table in Tables)
        {
            if (!spec.TryGetTable(table.Name, out var tableSpec))
            {
                Error("Data table has no matching spec.");
            }
            else
            {
                table.Validate(tableSpec);
            }
        }
        return Validator.Errors;
    }
}