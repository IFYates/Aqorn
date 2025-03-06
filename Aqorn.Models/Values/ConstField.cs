using Aqorn.Models.Data;

namespace Aqorn.Models.Values;

public record ConstField(string Name, IValue Value) : IDataField;