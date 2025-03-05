using Aqorn.Models.Values;
using System.Text.Json;

namespace Aqorn.Readers.Json.Values;

/// <summary>
/// Converts a JSON value to a typed value.
/// "^" Parent field reference
/// "<" Self field reference
/// "@" Parameter reference
/// "$" Raw SQL
/// </summary>
internal sealed class JsonFieldValue : FieldValue
{
    public JsonFieldValue(JsonElement json)
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
                Type = ValueType.String;
                Value = json.GetString() ?? string.Empty;
                if (Value.Length > 0)
                {
                    switch (Value[0])
                    {
                        case '^':
                            Type = ValueType.Parent;
                            Value = Value[1..];
                            break;
                        case '<':
                            Type = ValueType.Self;
                            Value = Value[1..];
                            break;
                        case '@':
                            Type = ValueType.Parameter;
                            break;
                        case '$':
                            Type = ValueType.Sql;
                            Value = Value[1..];
                            break;
                    }
                }
                break;
        }
    }
}