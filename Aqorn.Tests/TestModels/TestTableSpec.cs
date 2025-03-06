using Aqorn.Models.Spec;

namespace Aqorn.Tests.TestModels;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public record TestTableSpec(
    string Name,
    string? SchemaName,
    string TableName,
    bool IdentityInsert,
    ITableSpec[] Relationships,
    ITableSpec? Parent = null,
    ISpecSchema Schema = null!
) : ITableSpec
{
    public List<TestColumnSpec> Columns { get; } = [];
    IColumnSpec[] ITableSpec.Columns => Columns.ToArray();

    public TestTableSpec(string fullTableName, TestColumnSpec[] columns)
        : this(fullTableName, fullTableName.Split('.')[0], string.Join('.', fullTableName.Split('.')[1..]), false, [], null)
    {
        Columns = columns.ToList();
    }
}