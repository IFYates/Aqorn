using Aqorn.Models.Data;
using Aqorn.Models.DbModel;

namespace Aqorn.Models.Values;

public class SubqueryValue : IValue
{
    public FieldValue.ValueType Type => FieldValue.ValueType.String;

    public string? SchemaName { get; protected init; }
    public string TableName { get; protected init; } = null!;
    public string FieldName { get; protected init; } = null!;
    public IDataField[] FieldsSpec { get; protected init; } = [];
    public IDataField[] Fields { get; private set; } = [];

    protected SubqueryValue() { }

    public IValue? Resolve(DbDataRow row)
    {
        return new SubqueryValue
        {
            SchemaName = SchemaName,
            TableName = TableName,
            FieldName = FieldName,
            Fields = FieldsSpec.Select(f => new ConstField(f.Name, f.Value.Resolve(row)!)).ToArray()
        };
    }
}