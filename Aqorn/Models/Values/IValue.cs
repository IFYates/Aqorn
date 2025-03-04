namespace Aqorn.Models.Values;

internal interface IValue
{
    FieldValue.ValueType Type { get; }
}