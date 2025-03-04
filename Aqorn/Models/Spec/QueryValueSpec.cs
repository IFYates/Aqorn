using Aqorn.Models.Data;
using System.Collections.ObjectModel;

namespace Aqorn.Models.Spec;

internal class QueryValueSpec(IModel parent)
    : ValueBase(parent)
{
    public string TableName { get; protected init; } = null!;
    public string FieldName { get; protected init; } = null!;
    public ReadOnlyDictionary<string, FieldModel> Fields { get; protected init; } = null!;
}