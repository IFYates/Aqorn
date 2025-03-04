using Aqorn.Models;
using Aqorn.Models.Data;
using System.Text.Json;

namespace Aqorn.Readers.Json.Data;

/// <summary>
/// Reads a JSON data file and converts to models.
/// </summary>
internal class JsonDataFile : ModelBase, IDataSchema
{
    public TableModel[] Tables { get; }

    public JsonDataFile(string file)
        : base(null!, null!)
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