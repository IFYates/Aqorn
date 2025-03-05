using Aqorn.Models.Values;

namespace Aqorn.Models.Data;

/// <summary>
/// A named field with a value.
/// </summary>
internal interface IDataField
{
    string Name { get; }
    IValue Value { get; }
}