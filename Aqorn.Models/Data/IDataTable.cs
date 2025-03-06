namespace Aqorn.Models.Data;

/// <summary>
/// Models a named table of data.
/// </summary>
public interface IDataTable
{
    string Name { get; }
    IDataSchema Schema { get; }
    IDataRow[] Rows { get; }
}