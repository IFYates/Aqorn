using Aqorn.Models.Values;

namespace Aqorn.Models.Spec;

internal class FieldSpec(string name)
{
    public string Name { get; } = name;
    public FieldTypeSpec? ValueType { get; protected init; }
    public IValue? Value { get; protected init; }
}