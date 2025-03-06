using Aqorn.Models.Spec;
using Aqorn.Models.Values;

namespace Aqorn.Tests.TestModels;

public class TestColumnSpec(string name, FieldValue.ValueType type, bool required = true)
    : IColumnSpec
{
    public string Name { get; } = name;
    public IFieldTypeSpec? ValueType { get; } = new TestFieldTypeSpec(type, required);
    public IValue? DefaultValue { get; }
}
