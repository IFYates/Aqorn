using Aqorn.Models.Spec;

namespace Aqorn.Models.Data;

internal class TableModel(ModelBase parent, string name)
    : ModelBase(parent, name)
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