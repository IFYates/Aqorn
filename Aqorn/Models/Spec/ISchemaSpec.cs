namespace Aqorn.Models.Spec;

internal interface ISchemaSpec
{
    TableSpec[] Tables { get; }
}