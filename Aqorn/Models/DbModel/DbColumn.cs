using Aqorn.Models.Spec;

namespace Aqorn.Models.DbModel;

internal class DbColumn : ModelBase
{
    public DbColumn(DbTable table, FieldSpec spec)
        : base(table, spec.Name)
    {
        IsRequired = spec.ValueType?.IsRequired == true;
        Type = spec switch
        {
            { Value: FieldValue value } => value.Type,
            { Value: ConcatenatedValue } => FieldValue.ValueType.String,
            { ValueType: FieldTypeSpec type } => type.Type,
            _ => FieldValue.ValueType.Null
        };
    }

    public bool IsRequired { get; }
    public FieldValue.ValueType Type { get; }
}