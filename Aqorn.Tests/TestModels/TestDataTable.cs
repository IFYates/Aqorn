using Aqorn.Models.Data;

namespace Aqorn.Tests.TestModels;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public record TestDataTable : IDataTable
{
    public IDataSchema Schema { get; }
    public string Name { get; }
    public List<TestDataRow> Rows { get; } = [];
    IDataRow[] IDataTable.Rows => Rows.ToArray();

    public TestDataTable(TestTableSpec table)
    {
        Schema = null!;
        Name = table.Name;
    }
    public TestDataTable(TestTableSpec table, params string[][] rowData)
    {
        Schema = null!;
        Name = table.Name;
        Rows = rowData.Select(r => new TestDataRow(this, table.Columns.ToArray(), r)).ToList();
    }
}