using Aqorn.Models.Data;

namespace Aqorn.Models.Spec;

internal class QueryValueSpec
{
    public string? SchemaName { get; protected init; }
    public string TableName { get; protected init; } = null!;
    public string FieldName { get; protected init; } = null!;

    public FieldModel[] Fields { get; protected init; } = null!;
}