namespace Aqorn.Models;

internal class ModelValidator : IModelValidator
{
    private readonly List<string> _errors = [];
    public string[] Errors => _errors.ToArray();

    public void AddError(IModel model, string text)
    {
        _errors.Add((model.Path.Length > 0 ? model.Path + ": " : null) + text);
    }
}