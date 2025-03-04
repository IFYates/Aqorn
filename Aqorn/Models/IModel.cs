namespace Aqorn.Models;

internal interface IModel
{
    string Name { get; }
    IModel Parent { get; }
    string Path { get; }
}