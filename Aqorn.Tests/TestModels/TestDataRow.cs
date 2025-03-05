using Aqorn.Models.Data;
using Aqorn.Models.Spec;

namespace Aqorn.Tests.TestModels;

internal class TestDataRow(IDataTable table, IColumnSpec[] columns, string[] values)
    : IDataRow
{
    public IDataTable Table { get; } = table;
    public IDataField[] Fields { get; }
        = values.Zip(columns, (v, c) => new TestDataField(c, v)).ToArray();
    public IDataTable[] Relationships { get; } = [];
}