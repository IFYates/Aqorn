using Aqorn.Models.Spec;
using Aqorn.Models.Values;

namespace Aqorn.Models.DbModel;

public sealed class DbColumn(DbTable table, IColumnSpec spec)
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
                if (str != bool.TrueString && str != bool.FalseString)
                {
                    errors.Add("Value is not a boolean: " + str);
                    return FieldValue.Null;
                }
                value = FieldValue.Boolean(str == bool.TrueString);
                break;
            case FieldValue.ValueType.Number:
                if (!decimal.TryParse(str, out _))
                {
                    errors.Add("Value is not a number: " + str);
                    return FieldValue.Null;
                }
                value = FieldValue.Number(str);
                break;
            case FieldValue.ValueType.Parent: // Accept resolved value
            case FieldValue.ValueType.Self: // Accept resolved value
            case FieldValue.ValueType.Sql:
            case FieldValue.ValueType.String:
                break;
            default:
                errors.Add($"Unexpected value type ({Type}).");
                return FieldValue.Null;
        }

        if (ValueType?.Length < str.Length)
        {
            errors.Add($"Value is too long ({str.Length} > {ValueType.Length}).");
        }
        else if (ValueType?.Regex?.IsMatch(str) == false)
        {
            errors.Add($"Value does not match pattern ({ValueType.Regex}).");
        }
        return value;
    }
}