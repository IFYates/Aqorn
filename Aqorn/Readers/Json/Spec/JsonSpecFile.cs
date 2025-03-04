using Aqorn.Models.Spec;
using System.Text.Json;

namespace Aqorn.Readers.Json.Spec;

/// <summary>
/// Reads a JSON specification file and converts to spec models.
/// </summary>
internal class JsonSpecFile : ISchemaSpec
{
    public TableSpec[] Tables { get; }

    public JsonSpecFile(IErrorLog errors, string jsonSpecFile)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(jsonSpecFile);

        try
        {
            var json = File.ReadAllText(jsonSpecFile);
            var doc = JsonDocument.Parse(json, new JsonDocumentOptions { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip });

            if (doc.RootElement.ValueKind != JsonValueKind.Object)
            {
                errors.Add("Bad file structure.");
                Tables = [];
            }
            else
            {
                Tables = doc.RootElement.EnumerateObject()
                    .Select(t => new JsonTableSpec(errors.Step(t.Name), t.Name, t.Value)).ToArray();
            }
        }
        catch
        {
            errors.Add("Invalid JSON file.");
            Tables = [];
        }
    }
}