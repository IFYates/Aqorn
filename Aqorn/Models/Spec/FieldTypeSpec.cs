using Aqorn.Models.Values;
using System.Text.RegularExpressions;

namespace Aqorn.Models.Spec;

internal class FieldTypeSpec
{
    public bool IsRequired { get; protected init; }
    public FieldValue.ValueType Type { get; protected init; } = FieldValue.ValueType.Null;
    public Regex? Regex { get; protected init; }
    public int? Length { get; protected init; }

    public FieldTypeSpec(FieldValue.ValueType type)
    {
        Type = type;
        IsRequired = type != FieldValue.ValueType.Null;
    }
    public FieldTypeSpec()
    {
    }
}