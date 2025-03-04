using Aqorn.Models.Spec;
using Aqorn.Models.Values;

namespace Aqorn.Models.DbModel;

internal class DbColumn(DbTable table, FieldSpec spec)
{
    public DbTable Table { get; } = table;
    public string Name { get; } = spec.Name;

    public FieldTypeSpec? ValueType { get; } = spec.ValueType;
    public IValue? DefaultValue { get; } = spec.Value;
    public bool IsRequired => ValueType?.IsRequired == true;
    public FieldValue.ValueType Type
        => DefaultValue?.Type ?? ValueType?.Type ?? FieldValue.ValueType.Null;
}