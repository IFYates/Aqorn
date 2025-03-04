namespace Aqorn.Models.Data;

internal class TableRowModel(TableModel parent)
{
    public TableModel Table { get; } = parent;

    public FieldModel[] Fields { get; protected init; } = [];
    public TableModel[] Relationships { get; protected init; } = [];
}