namespace Aqorn.Models.Data;

/// <summary>
/// A row of data in a table.
/// </summary>
public interface IDataRow
{
    IDataTable Table { get; }
    IDataField[] Fields { get; }
    IDataTable[] Relationships { get; }
}