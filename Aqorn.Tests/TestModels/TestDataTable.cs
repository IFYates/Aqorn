using Aqorn.Models.Data;
using Aqorn.Models.Spec;

namespace Aqorn.Tests.TestModels;

internal class TestDataTable : IDataTable
{
    public string Name { get; }
    public IDataRow[] Rows { get; }

    public TestDataTable(ITableSpec table, params string[][] rowData)
    {
        Name = table.Name;
        Rows = rowData.Select(r => new TestDataRow(this, table.Columns.ToArray(), r)).ToArray();
    }
}