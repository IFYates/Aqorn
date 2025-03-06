using Aqorn.Models;
using Aqorn.Models.Data;
using System.Text.Json;

namespace Aqorn.Readers.Json.Data;

/// <summary>
/// Reads a JSON data file and converts to models.
/// </summary>
public sealed class JsonDataFileReader : IDataSchema
{
    public IDataField[] Parameters { get; }
    public IDataTable[] Tables { get; }

    public JsonDataFileReader(IErrorLog errors, string file)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(file);

        try
        {
            var json = File.ReadAllText(file);
            var doc = JsonDocument.Parse(json, new JsonDocumentOptions { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip });

            if (doc.RootElement.ValueKind != JsonValueKind.Object)
            {
                errors.Add("Bad file structure.");
                Parameters = [];
                Tables = [];
            }
            else
            {
                Parameters = doc.RootElement.EnumerateObject()
                    .Where(t => t.Name[0] == '@')
                    .Select(t => new JsonDataField(errors, t.Name, t.Value)).ToArray();

                Tables = doc.RootElement.EnumerateObject()
                    .Where(t => t.Name[0] != '@')
                    .Select(t => new JsonDataTable(errors, t.Name, t.Value, this)).ToArray();
            }
        }
        catch
        {
            errors.Add("Invalid JSON file.");
            Parameters = [];
            Tables = [];
        }
    }
}