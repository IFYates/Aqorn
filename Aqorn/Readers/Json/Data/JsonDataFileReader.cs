using Aqorn.Models;
using Aqorn.Models.Data;
using Aqorn.Models.Values;
using System.Text.Json;

namespace Aqorn.Readers.Json.Data;

/// <summary>
/// Reads a JSON data file and converts to models.
/// </summary>
public sealed class JsonDataFileReader : IDataSchema
{
    private Dictionary<string, IDataField> _parameters = [];
    public IDataField[] Parameters => _parameters.Values.ToArray();
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
                Tables = [];
            }
            else
            {
                _parameters = doc.RootElement.EnumerateObject()
                    .Where(t => t.Name[0] == '@')
                    .Select(t => new JsonDataField(errors, t.Name, t.Value))
                    .ToDictionary(p => p.Name, p => (IDataField)p);

                Tables = doc.RootElement.EnumerateObject()
                    .Where(t => t.Name[0] != '@')
                    .Select(t => new JsonDataTable(errors, t.Name, t.Value, this)).ToArray();
            }
        }
        catch
        {
            errors.Add("Invalid JSON file.");
            Tables = [];
        }
    }

    public void SetParameter(string name, string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        name = name[0] != '@' ? "@" + name : name;
        _parameters[name] = new ConstField(name, FieldValue.String(value));
    }
}