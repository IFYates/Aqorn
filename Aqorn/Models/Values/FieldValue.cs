using Aqorn.Models.DbModel;

namespace Aqorn.Models.Values;

/// <summary>
/// Models a typed value.
/// </summary>
internal class FieldValue : IValue
{
    public enum ValueType
    {
        Unknown,

        Null,
        Number,
        Boolean,
        Sql,
        String,

        Self,
        Parent,
        Parameter
    }
    public ValueType Type { get; protected init; }

    public string Value { get; protected init; } = null!;

    public static FieldValue Null { get; } = new() { Type = ValueType.Null };
    public static FieldValue Boolean(bool value) => new() { Type = ValueType.Boolean, Value = value.ToString() };
    public static FieldValue String(string? literal) => new() { Type = ValueType.String, Value = literal ?? string.Empty };
    public static FieldValue Number(string value) => new() { Type = ValueType.Number, Value = value };

    protected FieldValue() { }

    public IValue? Resolve(DbDataRow row)
    {
        return Type switch
        {
            ValueType.Null => Null,
            ValueType.Self => row.TryGetField(Value, out var field) ? field.Value : null,
            ValueType.Parent => row.ParentRow?.TryGetField(Value, out var field) == true ? field.Value : null,
            ValueType.Parameter => row.TryGetField(Value, out var parameter) ? parameter.Value : null,
            _ => this,
        };
    }

    public override string ToString()
        => Value;
}