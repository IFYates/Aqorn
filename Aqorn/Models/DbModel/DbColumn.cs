using Aqorn.Models.Spec;
using Aqorn.Models.Values;
using Aqorn.Readers;

namespace Aqorn.Models.DbModel;

internal sealed class DbColumn(DbTable table, IColumnSpec spec)
{
    public DbTable Table { get; } = table;
    public string Name { get; } = spec.Name;

    public IFieldTypeSpec? ValueType { get; } = spec.ValueType;
    public IValue? DefaultValue { get; } = spec.DefaultValue;
    public bool IsRequired { get; } = spec.ValueType?.IsRequired == true && spec.DefaultValue == null;
    public FieldValue.ValueType Type
        => ValueType?.Type ?? DefaultValue?.Type ?? FieldValue.ValueType.Unknown;

    public IValue Validate(IValue value, IErrorLog errors)
    {
        if (value is not FieldValue fv)
        {
            errors.Add("Invalid value.");
            return FieldValue.Null;
        }
        var str = fv.Value;
        switch (Type)
        {
            case FieldValue.ValueType.Boolean:
                if (str is not "0" and not "1")
                {
                    errors.Add("Value is not a boolean.");
                    return FieldValue.Null;
                }
                value = FieldValue.Boolean(str == "1");
                break;
            case FieldValue.ValueType.Number:
                if (!decimal.TryParse(str, out _))
                {
                    errors.Add("Value is not a number.");
                    return FieldValue.Null;
                }
                value = FieldValue.Number(str);
                break;
            case FieldValue.ValueType.Sql:
            case FieldValue.ValueType.String:
                break;
            default:
                errors.Add($"Unexpected value type ({Type}).");
                return FieldValue.Null;
        }

        if (ValueType?.Length < str.Length)
        {
            errors.Add("Value is too long.");
        }
        else if (ValueType?.Regex?.IsMatch(str) == false)
        {
            errors.Add("Value does not match pattern.");
        }
        return value;
    }
}