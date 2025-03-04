namespace Aqorn.Models;

internal class ConcatenatedValue(IModel parent)
    : ValueBase(parent)
{
    public FieldValue[] Values { get; protected init; } = [];
}