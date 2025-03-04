namespace Aqorn.Models;

internal abstract class ModelBase(ModelBase parent, string name)
    : IModel
{
    public string Name { get; } = name;
    public IModel Parent { get; } = parent;
    public string Path { get; } = name?.Length > 0
        ? (parent?.Path?.Length > 0 ? parent?.Path + "." : null) + (name.Contains('.') ? $"[{name}]" : name)
        : parent?.Path ?? string.Empty;

    protected virtual IModelValidator Validator { get; } = parent?.Validator!;
    protected void Error(string text) => Validator.AddError(this, text);
}