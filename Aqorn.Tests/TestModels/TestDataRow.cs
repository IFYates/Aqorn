using Aqorn.Models.Data;

namespace Aqorn.Tests.TestModels;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class TestDataRow(TestDataTable table, IEnumerable<TestColumnSpec> columns, params string[] values)
    : IDataRow
{
    public TestDataTable Table { get; } = table;
    IDataTable IDataRow.Table => Table;

    public List<TestDataField> Fields { get; }
        = columns.Select((c, i) => new TestDataField(c, values.Length > i ? values[i] : null!)).ToList();
    IDataField[] IDataRow.Fields => Fields.ToArray();

    public IDataTable[] Relationships { get; } = [];
}