using Aqorn.Models.Values;
using System.Text.RegularExpressions;

namespace Aqorn.Models.Spec;

/// <summary>
/// Describes a field type specification.
/// </summary>
internal interface IFieldTypeSpec
{
    bool IsRequired { get; }
    FieldValue.ValueType Type { get; }
    Regex? Regex { get; }
    int? Length { get; }
}