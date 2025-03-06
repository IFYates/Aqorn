using Aqorn.Models.Spec;
using System.Text.Json;

namespace Aqorn.Readers.Json.Spec;

/// <summary>
/// Reads a JSON specification file and converts to spec models.
/// </summary>
internal sealed class JsonSpecFileReader : ISpecSchema
{
    public IColumnSpec[] Parameters { get; }
    public ITableSpec[] Tables { get; }

    public JsonSpecFileReader(IErrorLog errors, string jsonSpecFile)
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
                Parameters = doc.RootElement.EnumerateObject()
                    .Where(t => t.Name[0] == '@')
                    .Select(t => new JsonParameterSpec(errors, t.Name, t.Value)).ToArray();

                Tables = doc.RootElement.EnumerateObject()
                    .Where(t => t.Name[0] != '@')
                    .Select(t => new JsonTableSpec(errors, t.Name, t.Value, this)).ToArray();
            }
        }
        catch
        {
            errors.Add("Invalid JSON file.");
            Tables = [];
        }
    }
}