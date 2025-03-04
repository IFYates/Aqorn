using Aqorn.Models.Spec;

namespace Aqorn.Models.Data;

internal interface IDataModel<T> : IModel
    where T : ISpecModel
{
    void Validate(T spec);
}