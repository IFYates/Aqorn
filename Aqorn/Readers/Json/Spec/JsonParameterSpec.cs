using Aqorn.Models;
using Aqorn.Models.Spec;
using System.Text.Json;

namespace Aqorn.Readers.Json.Spec;

internal class JsonParameterSpec : FieldSpec
{
    public JsonParameterSpec(IModel parent, string name, JsonElement json)
        : base(parent, name)
    {
        switch (json.ValueKind)
        {
            case JsonValueKind.String:
                ValueType = new FieldTypeSpec(this, json.GetString()!);
                break;
            default:
                Error($"Invalid parameter spec.");
                break;
        }
    }
}