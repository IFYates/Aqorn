using Aqorn.Models;
using System.Text.Json;

namespace Aqorn.Readers.Json;

/// <summary>
/// Models a value.
/// "^" Parent field reference
/// "<" Self field reference
/// "@" Parameter reference
/// "$" Raw SQL
/// </summary>
internal class JsonFieldValue : FieldValue
{
    public JsonFieldValue(IModel parent, JsonElement json, bool literal = false)
        : base(parent)
    {
        switch (json.ValueKind)
        {
            case JsonValueKind.Null:
                Type = ValueType.Null;
                break;
            case JsonValueKind.Number:
                Type = ValueType.Number;
                Value = json.GetRawText();
                break;
            case JsonValueKind.True or JsonValueKind.False:
                Type = ValueType.Boolean;
                Value = json.ValueKind == JsonValueKind.True ? "1" : "0";
                break;
            case JsonValueKind.String:
                var value = json.GetString();
                if (string.IsNullOrEmpty(value))
                {
                    Type = ValueType.Null;
                    return;
                }

                Type = ValueType.String;
                Value = value;
                if (!literal && value.Length > 0)
                {
                    Type = value[0] switch
                    {
                        '^' => ValueType.Parent,
                        '<' => ValueType.Self,
                        '@' => ValueType.Parameter,
                        '$' => ValueType.Sql,
                        _ => ValueType.String
                    };
                    if (Type != ValueType.String)
                    {
                        Value = value[1..];
                    }
                }
                break;

            default:
                Error("Invalid value type.");
                break;
        }
    }
}