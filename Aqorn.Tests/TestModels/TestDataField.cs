using Aqorn.Models.Data;
using Aqorn.Models.Spec;
using Aqorn.Models.Values;

namespace Aqorn.Tests.TestModels;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class TestDataField(string name, string value)
    : IDataField
{
    public string Name { get; } = name;
    public IValue Value { get; set; } = value != null ? FieldValue.String(value) : FieldValue.Null;

    public TestDataField(IColumnSpec column, string value)
        : this(column.Name, value)
    {
    }
}