using Aqorn.Models.Data;
using Aqorn.Models.DbModel;

namespace Aqorn.Models.Values;

internal abstract class SubqueryValue : IValue
{
    public FieldValue.ValueType Type => FieldValue.ValueType.String;

    public string? SchemaName { get; protected init; }
    public string TableName { get; protected init; } = null!;
    public string FieldName { get; protected init; } = null!;
    public IDataField[] FieldsSpec { get; protected init; } = [];
    public IDataField[] Fields { get; private set; } = [];

    public IValue? Resolve(DbDataRow row)
    {
        Fields = FieldsSpec.Select(f => new LiteralField(f.Name, f.Value.Resolve(row)!)).ToArray();
        return this;
    }

    private record LiteralField(string Name, IValue Value) : IDataField;
}