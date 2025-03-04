using System.Text.RegularExpressions;

namespace Aqorn.Models.Spec;

internal partial class FieldTypeSpec : ModelBase, ISpecModel
{
    public bool IsRequired { get; }
    public FieldValue.ValueType Type { get; } = FieldValue.ValueType.Null;
    public Regex? Regex { get; set; }
    public int? Length { get; }

    public FieldTypeSpec(IModel parent, FieldValue.ValueType type)
        : base(parent, null!)
    {
        Type = type;
        IsRequired = type != FieldValue.ValueType.Null;
    }
    public FieldTypeSpec(IModel parent, string type)
        : base(parent, null!)
    {
        if (string.IsNullOrEmpty(type))
        {
            Error($"Missing type for '{parent.Name}'");
            return;
        }

        var match = TypeFormat().Match(type[1..]);
        if (!(type[0] is '!' or '?') || !match.Success)
        {
            Error($"Invalid type '{type}'.");
            return;
        }

        IsRequired = type[0] == '!';
        type = match.Groups[1].Value;
        if (match.Groups[2].Success)
        {
            Length = int.Parse(match.Groups[2].Value);
            type = type[0..^(match.Groups[2].Value.Length + 2)];
        }

        if (type[0] == '/')
        {
            Type = FieldValue.ValueType.Regex;
            Regex = new(type);
        }
        else
        {
            Type = type switch
            {
                // TODO: "binary" => FieldValue.ValueType.Null,
                "bool" => FieldValue.ValueType.Boolean,
                "number" => FieldValue.ValueType.Number,
                "string" => FieldValue.ValueType.String,
                _ => FieldValue.ValueType.Null
            };
        }

        if (Type == FieldValue.ValueType.Null)
        {
            Error($"Invalid type '{type}'.");
        }
    }

    [GeneratedRegex(@"^(binary|bool|number|string(?:\((\d+)\))?|/[^/]+/\w*)$")]
    public static partial Regex TypeFormat();
}