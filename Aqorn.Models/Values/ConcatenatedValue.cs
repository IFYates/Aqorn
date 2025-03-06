using Aqorn.Models.DbModel;

namespace Aqorn.Models.Values;

/// <summary>
/// Models a concatenated value.
/// </summary>
public abstract class ConcatenatedValue : IValue
{
    public FieldValue.ValueType Type => FieldValue.ValueType.String;
    public IValue[] Values { get; protected init; } = [];

    public IValue? Resolve(DbDataRow row)
        => FieldValue.String(string.Join("", Values.Select(v => v.Resolve(row))));
}