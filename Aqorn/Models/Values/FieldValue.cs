namespace Aqorn.Models.Values;

/// <summary>
/// Models a typed value.
/// </summary>
internal class FieldValue : IValue
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

    public static FieldValue Null { get; } = new() { Type = ValueType.Null };
    public static FieldValue String(string value) => new() { Type = ValueType.String, Value = value };
    public static FieldValue Number(string value) => new() { Type = ValueType.Number, Value = value };
}