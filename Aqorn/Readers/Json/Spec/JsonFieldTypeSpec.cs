using Aqorn.Models.Spec;
using Aqorn.Models.Values;
using System.Text.RegularExpressions;

namespace Aqorn.Readers.Json.Spec;

internal partial class JsonFieldTypeSpec : FieldTypeSpec
{
    public JsonFieldTypeSpec(IErrorLog errors, string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            errors.Add("Missing type.");
            return;
        }

        var match = TypeFormat().Match(json);
        if (!(json[0] is '!' or '?') || !match.Success)
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
            Type = FieldValue.ValueType.Regex;
            Regex = new(type[1..^1]);
            return;
        }

        Type = type switch
        {
            // TODO: "binary" => FieldValue.ValueType.Null,
            "bool" => FieldValue.ValueType.Boolean,
            "number" => FieldValue.ValueType.Number,
            "string" => FieldValue.ValueType.String,
            _ => FieldValue.ValueType.Null
        };
        if (Type == FieldValue.ValueType.Null)
        {
            errors.Add($"Invalid type '{type}'.");
        }
    }

    [GeneratedRegex(@"^[!?](binary|bool|number|string(?:\((\d+)\))?|/[^/]+/)$")]
    public static partial Regex TypeFormat();
}