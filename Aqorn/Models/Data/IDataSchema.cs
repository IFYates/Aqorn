namespace Aqorn.Models.Data;

/// <summary>
/// The base model of a data file.
/// </summary>
internal interface IDataSchema
{
    IDataTable[] Tables { get; }
}