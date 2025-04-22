using Aqorn.Models;
using Aqorn.Models.Spec;
using Aqorn.Models.Values;
using System.Text.Json;

namespace Aqorn.Readers.Json.Spec;

public sealed class JsonParameterSpec(IErrorLog errors, string name, JsonElement json)
    : JsonColumnSpec(errors, name, json)
{
}