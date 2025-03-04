namespace Aqorn.Models.Spec;

internal interface ISchemaSpec : ISpecModel, IModelValidator
{
    TableSpec[] Tables { get; }
}