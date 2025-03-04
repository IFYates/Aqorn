namespace Aqorn.Models.Data;

internal class TableModel(string name)
{
    public string Name { get; } = name;
    public TableRowModel[] Rows { get; protected init; } = [];
}