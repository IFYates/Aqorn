namespace Aqorn.Models.Data;

/// <summary>
/// Models a named table of data.
/// </summary>
internal interface IDataTable
{
    string Name { get; }
    IDataRow[] Rows { get; }
}