using Aqorn.Models.Data;
using System.Text.Json;

namespace Aqorn.Readers.Json.Data;

/// <summary>
/// Reads a JSON data file and converts to models.
/// </summary>
internal class JsonDataFile : IDataSchema
{
    public TableModel[] Tables { get; }

    public JsonDataFile(IErrorLog errors, string file)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(file);

        try
        {
            var json = File.ReadAllText(file);
            var doc = JsonDocument.Parse(json, new JsonDocumentOptions { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip });

            if (doc.RootElement.ValueKind != JsonValueKind.Object)
            {
                errors.Add("Bad file structure.");
                Tables = [];
            }
            else
            {
                Tables = doc.RootElement.EnumerateObject()
                    .Select(t => new JsonTableModel(errors.Step(t.Name), t.Name, t.Value)).ToArray();
            }
        }
        catch
        {
            errors.Add("Invalid JSON file.");
            Tables = [];
        }
    }
}