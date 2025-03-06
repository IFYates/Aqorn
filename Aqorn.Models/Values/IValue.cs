using Aqorn.Models.DbModel;

namespace Aqorn.Models.Values;

public interface IValue
{
    FieldValue.ValueType Type { get; }
    IValue? Resolve(DbDataRow row);
}