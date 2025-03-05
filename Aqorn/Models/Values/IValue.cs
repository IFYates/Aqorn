using Aqorn.Models.DbModel;

namespace Aqorn.Models.Values;

internal interface IValue
{
    FieldValue.ValueType Type { get; }
    IValue? Resolve(DbDataRow row);
}