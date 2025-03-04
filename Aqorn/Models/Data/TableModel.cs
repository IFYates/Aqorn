using Aqorn.Models.Spec;

namespace Aqorn.Models.Data;

internal class TableModel(IModel parent, string name)
    : ModelBase(parent, name), IDataModel<TableSpec>
{
    public TableRowModel[] Rows { get; protected init; } = [];

    public TableSpec? Spec { get; private set; }
    public void Validate(TableSpec spec)
    {
        Spec = spec;
        foreach (var row in Rows)
        {
            row.Validate(spec);
        }
    }
}