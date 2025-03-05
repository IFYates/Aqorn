using Aqorn.Models.Spec;
using Aqorn.Models.Values;
using System.Text.Json;

namespace Aqorn.Readers.Json.Spec;

internal sealed class JsonParameterSpec : IColumnSpec
{
    public string Name { get; }
    public IFieldTypeSpec? ValueType { get; }
    public IValue? DefaultValue { get; }

    public JsonParameterSpec(IErrorLog errors, string name, JsonElement json)
    {
        Name = name;

        errors = errors.Step(name);
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