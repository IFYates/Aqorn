namespace Aqorn.Models.Spec;

/// <summary>
/// Represents a named table specification.
/// </summary>
public interface ITableSpec
{
    ITableSpec? Parent { get; }
    string Name { get; }
    string? SchemaName { get; }
    string TableName { get; }

    bool IdentityInsert { get; }
    IColumnSpec[] Columns { get; }
    ITableSpec[] Relationships { get; }
}