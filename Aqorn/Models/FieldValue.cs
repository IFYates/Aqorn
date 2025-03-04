namespace Aqorn.Models;

/// <summary>
/// Models a value.
/// </summary>
internal class FieldValue(IModel parent)
    : ValueBase(parent)
{
    public enum ValueType
    {
        Null,
        Number,
        Boolean,
        String,
        Regex,

        Self,
        Parent,
        Parameter,
        Sql
    }
    public ValueType Type { get; protected init; }

    public string Value { get; protected init; } = null!;
}