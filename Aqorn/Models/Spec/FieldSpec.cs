namespace Aqorn.Models.Spec;

internal class FieldSpec(ModelBase parent, string name)
    : ModelBase(parent, name), ISpecModel
{
    public FieldTypeSpec? ValueType { get; protected init; }
    public ValueBase? Value { get; protected init; }
}