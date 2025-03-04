using Aqorn.Models;
using Aqorn.Models.Spec;
using System.Text.Json;

namespace Aqorn.Readers.Json.Spec;

/// <summary>
/// Reads a JSON specification file and converts to spec models.
/// </summary>
internal class JsonSpecFile : ModelBase, ISchemaSpec
{
    public TableSpec[] Tables { get; }

    public JsonSpecFile(string jsonSpecFile)
        : base(null!, null!)
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

    #region IModelValidator

    protected override IModelValidator Validator => this;

    private readonly List<string> _errors = [];
    public string[] Errors => _errors.ToArray();

    public void AddError(IModel model, string text)
    {
        _errors.Add((model.Path.Length > 0 ? model.Path + ": " : null) + text);
    }

    #endregion IModelValidator
}