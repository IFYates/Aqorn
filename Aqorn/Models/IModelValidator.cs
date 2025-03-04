namespace Aqorn.Models;

internal interface IModelValidator
{
    void AddError(IModel model, string text);
    string[] Errors { get; }
}