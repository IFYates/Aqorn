namespace Aqorn.Models.Data;

/// <summary>
/// Models a named table of data.
/// </summary>
public interface IDataTable
{
    IDataSchema Schema { get; }
    string Name { get; }
    IDataRow[] Rows { get; }
}