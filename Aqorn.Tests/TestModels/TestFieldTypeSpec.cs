using Aqorn.Models.Spec;
using Aqorn.Models.Values;
using System.Text.RegularExpressions;

namespace Aqorn.Tests.TestModels;

public record TestFieldTypeSpec(
    FieldValue.ValueType Type,
    bool IsRequired = true,
    Regex? Regex = null,
    int? Length = null
) : IFieldTypeSpec;