namespace Aqorn.Models.Values;

/// <summary>
/// Models a concatenated value.
/// </summary>
internal class ConcatenatedValue : IValue
{
    public FieldValue.ValueType Type => FieldValue.ValueType.String;
    public FieldValue[] Values { get; protected init; } = [];

    public string SourcePath => throw new NotImplementedException();
}