using Aqorn.Models.Spec;
using System.Text.Json;

namespace Aqorn.Readers.Json.Spec;

internal class JsonParameterSpec : FieldSpec
{
    public JsonParameterSpec(IErrorLog errors, string name, JsonElement json)
        : base(name)
    {
        switch (json.ValueKind)
        {
            case JsonValueKind.String:
                ValueType = new JsonFieldTypeSpec(errors, json.GetString()!);
                break;
            default:
                errors.Add("Invalid parameter spec.");
                break;
        }
    }
}