namespace Aqorn.Models.Spec;

/// <summary>
/// Represents a named table specification.
/// </summary>
internal interface ITableSpec
{
    ITableSpec? Parent { get; }
    ISpecSchema Schema { get; }
    string Name { get; }
    string? SchemaName { get; }
    string TableName { get; }

    bool IdentityInsert { get; }
    IEnumerable<IColumnSpec> Columns { get; }
    ITableSpec[] Relationships { get; }
}