namespace Aqorn.Models;

internal abstract class ModelBase(IModel parent, string name, IModelValidator? validator = null)
    : IModel
{
    public virtual IModelValidator Validator { get; } = validator ?? parent?.Validator ?? throw new ArgumentNullException(nameof(validator));
    public string Name { get; } = name;
    public IModel Parent { get; } = parent;
    public string Path { get; } = name?.Length > 0
        ? (parent?.Path?.Length > 0 ? parent?.Path + "." : null) + (name.Contains('.') ? $"[{name}]" : name)
        : parent?.Path ?? string.Empty;

    protected void Error(string text)
        => Validator.AddError(this, text);
}