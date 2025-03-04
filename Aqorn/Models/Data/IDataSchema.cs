namespace Aqorn.Models.Data;

internal interface IDataSchema
{
    TableModel[] Tables { get; }
}