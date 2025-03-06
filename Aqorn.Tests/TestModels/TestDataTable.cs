using Aqorn.Models.Data;
using Aqorn.Models.Spec;

namespace Aqorn.Tests.TestModels;

public class TestDataTable : IDataTable
{
    public IDataSchema Schema { get; }
    public string Name { get; }
    public IDataRow[] Rows { get; }

    public TestDataTable(ITableSpec table, params string[][] rowData)
    {
        Schema = null!;
        Name = table.Name;
        Rows = rowData.Select(r => new TestDataRow(this, table.Columns.ToArray(), r)).ToArray();
    }
}