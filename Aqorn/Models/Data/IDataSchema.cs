namespace Aqorn.Models.Data;

internal interface IDataSchema : IModelValidator
{
    TableModel[] Tables { get; }
}