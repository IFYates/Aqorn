using Aqorn.Models.Spec;
using Aqorn.Models.Values;
using System.Text.RegularExpressions;

namespace Aqorn.Readers.Json.Spec;

internal sealed partial class JsonFieldTypeSpec : IFieldTypeSpec
{
    public bool IsRequired { get; }
    public FieldValue.ValueType Type { get; }
    public Regex? Regex { get; }
    public int? Length { get; }

    public JsonFieldTypeSpec(IErrorLog errors, string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            errors.Add("Missing type.");
            return;
        }

        var match = TypeFormat().Match(json);
        if (!match.Success)
        {
            errors.Add($"Invalid type '{json}'.");
            return;
        }

        IsRequired = json[0] == '!';
        var type = match.Groups[1].Value;
        if (match.Groups[2].Success)
        {
            Type = FieldValue.ValueType.String;
            Length = int.Parse(match.Groups[2].Value);
            return;
        }

        if (type[0] == '/')
        {
            Type = FieldValue.ValueType.String;
            Regex = new(type[1..^1]);
            return;
        }

        Type = type switch
        {
            // TODO: "binary" => FieldValue.ValueType.Null,
            "bool" => FieldValue.ValueType.Boolean,
            "number" => FieldValue.ValueType.Number,
            "string" => FieldValue.ValueType.String,
            _ => FieldValue.ValueType.Unknown
        };
        if (Type == FieldValue.ValueType.Unknown)
        {
            errors.Add($"Invalid type '{type}'.");
        }
    }

    [GeneratedRegex(@"^[!?](binary|bool|number|string(?:\((\d+)\))?|/[^/]+/)$")]
    public static partial Regex TypeFormat();
}