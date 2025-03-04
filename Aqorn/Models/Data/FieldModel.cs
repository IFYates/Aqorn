using Aqorn.Models.Values;

namespace Aqorn.Models.Data;

/// <summary>
/// A named field with a value.
/// </summary>
internal class FieldModel
{
    public string Name { get; }
    public TableRowModel Row { get; }
    public TableModel Table => Row.Table;

    public IValue Value { get; protected set; } = null!;

    public FieldModel(TableRowModel parent, string name)
    {
        Name = name;
        Row = parent;
    }
    public FieldModel(IValue parent, string name)
    {
        Name = name;
        Row = null!; // TODO
    }
}