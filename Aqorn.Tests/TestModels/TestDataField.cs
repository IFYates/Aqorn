using Aqorn.Models.Data;
using Aqorn.Models.Spec;
using Aqorn.Models.Values;

namespace Aqorn.Tests.TestModels;

internal class TestDataField(IColumnSpec column, string value)
    : IDataField
{
    public string Name { get; } = column.Name;
    public IValue Value { get; } = FieldValue.String(value);
}