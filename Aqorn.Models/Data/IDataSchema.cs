namespace Aqorn.Models.Data;

/// <summary>
/// The base model of a data file.
/// </summary>
public interface IDataSchema
{
    IDataField[] Parameters { get; }
    IDataTable[] Tables { get; }
}