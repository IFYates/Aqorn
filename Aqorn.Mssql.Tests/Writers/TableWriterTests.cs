using Aqorn.Models.DbModel;
using Aqorn.Models.Values;
using Aqorn.Tests.TestModels;
using System.Text;

namespace Aqorn.Mssql.Writers.Tests;

[TestClass]
public sealed class TableWriterTests
{
    [TestMethod]
    public void GenerateSql__Writes_basic_table_insert()
    {
        // Arrange
        var spec = new TestTableSpec("schema.Table",
            [
                new TestColumnSpec("Id", FieldValue.ValueType.Number, true),
                new TestColumnSpec("Name", FieldValue.ValueType.String, false),
            ]);
        var data = new TestDataTable(spec,
            [
                ["1", "One"],
                ["2", "Two"],
            ]);

        var errors = new TestErrorLog();
        var table = new DbTable(errors, (DbDataset)null!, spec);
        table.AddData(errors, data);

        var sb = new StringBuilder();
        var writer = new TableWriter();

        // Act
        writer.GenerateSql(table, sb);
        var result = sb.ToString().Trim();

        // Assert
        Assert.AreEqual(@"INSERT INTO [schema].[Table]([Id], [Name]) VALUES
    (1, 'One'),
    (2, 'Two')
GO -- 2", result);
        Assert.AreEqual(0, errors.Errors.Count);
    }

    [TestMethod]
    public void GenerateSql__Skips_unused_columns()
    {
        // Arrange
        var spec = new TestTableSpec("schema.Table",
            [
                new TestColumnSpec("Id", FieldValue.ValueType.Number, true),
                new TestColumnSpec("Name", FieldValue.ValueType.String, false),
            ]);
        var data = new TestDataTable(spec,
            [
                ["1"],
                ["2"],
            ]);

        var errors = new TestErrorLog();
        var table = new DbTable(errors, (DbDataset)null!, spec);
        table.AddData(errors, data);

        var sb = new StringBuilder();
        var writer = new TableWriter();

        // Act
        writer.GenerateSql(table, sb);
        var result = sb.ToString().Trim();

        // Assert
        Assert.AreEqual(@"INSERT INTO [schema].[Table]([Id]) VALUES
    (1),
    (2)
GO -- 2", result);
        Assert.AreEqual(0, errors.Errors.Count);
    }

    [TestMethod]
    public void GenerateSql__Outputs_in_groups_of_columns()
    {
        // Arrange
        var spec = new TestTableSpec("schema.Table",
            [
                new TestColumnSpec("Id", FieldValue.ValueType.Number, true),
                new TestColumnSpec("Name", FieldValue.ValueType.String, false),
            ]);
        var data = new TestDataTable(spec,
            [
                ["1"],
                ["2", "Two"],
            ]);

        var errors = new TestErrorLog();
        var table = new DbTable(errors, (DbDataset)null!, spec);
        table.AddData(errors, data);

        var sb = new StringBuilder();
        var writer = new TableWriter();

        // Act
        writer.GenerateSql(table, sb);
        var result = sb.ToString().Trim();

        // Assert
        Assert.AreEqual(@"INSERT INTO [schema].[Table]([Id]) VALUES
    (1)
GO -- 1
INSERT INTO [schema].[Table]([Id], [Name]) VALUES
    (2, 'Two')
GO -- 2", result);
        Assert.AreEqual(0, errors.Errors.Count);
    }
}