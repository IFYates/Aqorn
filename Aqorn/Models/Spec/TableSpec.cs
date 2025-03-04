namespace Aqorn.Models.Spec;

internal class TableSpec(string name)
{
    public string Name { get; } = name;
    public string? SchemaName { get; protected init; }
    public string TableName { get; protected init; } = null!;

    public bool IdentityInsert { get; protected init; }
    public FieldSpec[] Fields { get; protected init; } = [];
    public TableSpec[] Relationships { get; protected init; } = [];
}