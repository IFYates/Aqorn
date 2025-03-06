using Aqorn.Models.Spec;

namespace Aqorn.Tests.TestModels;

public record TestTableSpec(
    string Name,
    string? SchemaName,
    string TableName,
    bool IdentityInsert,
    IEnumerable<IColumnSpec> Columns,
    ITableSpec[] Relationships,
    ITableSpec? Parent = null,
    ISpecSchema Schema = null!
) : ITableSpec
{
    public TestTableSpec(string fullTableName, IEnumerable<IColumnSpec> columns)
        : this(fullTableName, fullTableName.Split('.')[0], string.Join('.', fullTableName.Split('.')[1..]), false, columns, [], null)
    { }
}