using Aqorn.Models.Spec;

namespace Aqorn.Models.Data;

internal class FieldModel(IModel parent, string name)
    : ModelBase(parent, name), IDataModel<FieldSpec>
{
    public ValueBase Value { get; protected set; } = null!;

    public void Validate(FieldSpec spec)
    {
        var type = spec.ValueType;
        if (Value is FieldValue fv && type != null)
        {
            var value = fv.Value;
            if ((string.IsNullOrEmpty(value) || fv.Type == FieldValue.ValueType.Null)
                && type.Type != FieldValue.ValueType.Null)
            {
                Error("Value is required.");
                return;
            }

            var valueMatch = type.Type switch
            {
                // TODO: FieldValue.ValueType.Binary => null,
                FieldValue.ValueType.Regex => type.Regex!.IsMatch(value),
                FieldValue.ValueType.Boolean => bool.TryParse(value, out _),
                FieldValue.ValueType.Number => decimal.TryParse(value, out _),
                FieldValue.ValueType.String => !type.Length.HasValue || value.Length <= type.Length.Value,
                _ => false,
            };
            if (!valueMatch)
            {
                Error($"Invalid value '{value}' ({type.Type}).");
            }
        }
        else if (Value is ConcatenatedValue && type?.Type is not null and not FieldValue.ValueType.String)
        {
            Error($"Concatenated value can not replace {type.Type}.");
            // TODO: Validate concatenated values
        }

        // TODO: Self, Parent, Parameter
    }
}