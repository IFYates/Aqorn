namespace Aqorn.Models;

internal interface IModel
{
    IModelValidator Validator { get; }
    string Name { get; }
    IModel Parent { get; }
    string Path { get; }
}