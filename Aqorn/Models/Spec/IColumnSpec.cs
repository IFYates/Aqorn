using Aqorn.Models.Values;

namespace Aqorn.Models.Spec;

/// <summary>
/// Represents a named table column specification.
/// </summary>
internal interface IColumnSpec
{
    string Name { get; }
    IFieldTypeSpec? ValueType { get; }
    IValue? DefaultValue { get; }
}