using Aqorn.Models;
using Aqorn.Models.DbModel;
using Aqorn.Models.Values;
using Aqorn.Tests.TestModels;
using System.Text;

namespace Aqorn.Mssql.Writers.Tests;

[TestClass]
public sealed class TableWriterTests
{
    private class Options : IOptions
    {
        public int InsertBatchSize { get; init; } = 100;
    }

    private static readonly TestTableSpec SPEC = new("schema.Table",
        [
            new TestColumnSpec("Id", FieldValue.ValueType.Number, true),
            new TestColumnSpec("Name", FieldValue.ValueType.String, false),
            new TestColumnSpec("Active", FieldValue.ValueType.Boolean, false),
        ]);
    private static readonly TestDataTable DATA = new(SPEC,
        [
            ["1", "One"],
            ["2", "Two"],
        ]);

    [TestMethod]
    public void GenerateSql__Does_nothing_if_no_data()
    {
        // Arrange
        var errors = new TestErrorLog();
        var table = new DbTable(errors, (DbDataset)null!, SPEC);

        var writer = new TableWriter(new Options());

        // Act
        var sb = new StringBuilder();
        writer.GenerateSql(table, sb);
        var result = sb.ToString().Trim();

        // Assert
        Assert.AreEqual(0, result.Length);
        Assert.AreEqual(0, errors.Errors.Count);
    }

    [TestMethod]
    public void GenerateSql__Writes_basic_table_insert()
    {
        // Arrange
        var errors = new TestErrorLog();
        var table = new DbTable(errors, (DbDataset)null!, SPEC);
        table.AddData(errors, DATA);

        var writer = new TableWriter(new Options());

        // Act
        var sb = new StringBuilder();
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
    public void GenerateSql__Table_schema_is_optional()
    {
        // Arrange
        var spec = SPEC with
        {
            SchemaName = null
        };
        var data = new TestDataTable(spec,
            [
                ["1", "One"],
                ["2", "Two"],
            ]);

        var errors = new TestErrorLog();
        var table = new DbTable(errors, (DbDataset)null!, spec);
        table.AddData(errors, data);

        var writer = new TableWriter(new Options());

        // Act
        var sb = new StringBuilder();
        writer.GenerateSql(table, sb);
        var result = sb.ToString().Trim();

        // Assert
        Assert.AreEqual(@"INSERT INTO [Table]([Id], [Name]) VALUES
    (1, 'One'),
    (2, 'Two')
GO -- 2", result);
        Assert.AreEqual(0, errors.Errors.Count);
    }

    [TestMethod]
    public void GenerateSql__Skips_unused_columns()
    {
        // Arrange
        var data = new TestDataTable(SPEC,
            [
                ["1", null!],
                ["2"],
            ]);

        var errors = new TestErrorLog();
        var table = new DbTable(errors, (DbDataset)null!, SPEC);
        table.AddData(errors, data);

        var writer = new TableWriter(new Options());

        // Act
        var sb = new StringBuilder();
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
    public void GenerateSql__Supports_boolean()
    {
        // Arrange
        var data = new TestDataTable(SPEC,
        [
            ["1", "One", "True"],
            ["2", "Two", "False"],
        ]);

        var errors = new TestErrorLog();
        var table = new DbTable(errors, (DbDataset)null!, SPEC);
        table.AddData(errors, data);

        var writer = new TableWriter(new Options());

        // Act
        var sb = new StringBuilder();
        writer.GenerateSql(table, sb);
        var result = sb.ToString().Trim();

        // Assert
        Assert.AreEqual(@"INSERT INTO [schema].[Table]([Id], [Name], [Active]) VALUES
    (1, 'One', 1),
    (2, 'Two', 0)
GO -- 2", result);
        Assert.AreEqual(0, errors.Errors.Count);
    }

    [TestMethod]
    public void GenerateSql__Supports_explicit_NULL()
    {
        // Arrange
        var data = new TestDataTable(SPEC,
        [
            [null!]
        ]);

        var errors = new TestErrorLog();
        var table = new DbTable(errors, (DbDataset)null!, SPEC);
        table.AddData(errors, data);

        var writer = new TableWriter(new Options());

        // Act
        var sb = new StringBuilder();
        writer.GenerateSql(table, sb);
        var result = sb.ToString().Trim();

        // Assert
        Assert.AreEqual(@"INSERT INTO [schema].[Table]([Id]) VALUES
    (NULL)
GO -- 1", result);
        Assert.AreEqual(0, errors.Errors.Count);
    }

    [TestMethod]
    public void GenerateSql__Outputs_in_groups_of_columns()
    {
        // Arrange
        var data = new TestDataTable(SPEC,
            [
                ["1"],
                ["2", "Two"],
            ]);

        var errors = new TestErrorLog();
        var table = new DbTable(errors, (DbDataset)null!, SPEC);
        table.AddData(errors, data);

        var writer = new TableWriter(new Options());

        // Act
        var sb = new StringBuilder();
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

    [TestMethod]
    public void GenerateSql__Can_do_identity_insert()
    {
        // Arrange
        var spec = SPEC with
        {
            IdentityInsert = true
        };
        var data = new TestDataTable(spec,
        [
            ["1", "One"],
            ["2", "Two"],
        ]);

        var errors = new TestErrorLog();
        var table = new DbTable(errors, (DbDataset)null!, spec);
        table.AddData(errors, data);

        var writer = new TableWriter(new Options());

        // Act
        var sb = new StringBuilder();
        writer.GenerateSql(table, sb);
        var result = sb.ToString().Trim();

        // Assert
        Assert.AreEqual(@"SET IDENTITY_INSERT [schema].[Table] ON
GO
INSERT INTO [schema].[Table]([Id], [Name]) VALUES
    (1, 'One'),
    (2, 'Two')
GO -- 2
SET IDENTITY_INSERT [schema].[Table] OFF
GO", result);
        Assert.AreEqual(0, errors.Errors.Count);
    }

    [TestMethod]
    public void GenerateSql__Outputs_in_batches()
    {
        // Arrange
        var data = new TestDataTable(SPEC,
            [
                ["1", "One"],
                ["2", "Two"],
            ]);

        var errors = new TestErrorLog();
        var table = new DbTable(errors, (DbDataset)null!, SPEC);
        table.AddData(errors, data);

        var writer = new TableWriter(new Options { InsertBatchSize = 1 });

        // Act
        var sb = new StringBuilder();
        writer.GenerateSql(table, sb);
        var result = sb.ToString().Trim();

        // Assert
        Assert.AreEqual(@"INSERT INTO [schema].[Table]([Id], [Name]) VALUES
    (1, 'One')
GO -- 1
INSERT INTO [schema].[Table]([Id], [Name]) VALUES
    (2, 'Two')
GO -- 2", result);
        Assert.AreEqual(0, errors.Errors.Count);
    }

    [TestMethod]
    public void GenerateSql__Subquery_can_check_other_tables()
    {
        // Arrange
        var data = new TestDataTable(SPEC);
        var row = new TestDataRow(data, SPEC.Columns);
        row.Fields[0].Value = new TestSubqueryValue("ext", "Data", "Key",
            [
                new TestDataField("Field1", "Value1"),
                new TestDataField("Field2", "Value2")
            ]);
        data.Rows.Add(row);

        var errors = new TestErrorLog();
        var table = new DbTable(errors, (DbDataset)null!, SPEC);
        table.AddData(errors, data);

        var writer = new TableWriter(new Options { InsertBatchSize = 1 });

        // Act
        var sb = new StringBuilder();
        writer.GenerateSql(table, sb);
        var result = sb.ToString().Trim();

        // Assert
        Assert.AreEqual(@"INSERT INTO [schema].[Table]([Id]) VALUES
    ((SELECT TOP 1 [Key] FROM [ext].[Data] WHERE [Field1] = 'Value1' AND [Field2] = 'Value2'))
GO -- 1", result);
        Assert.AreEqual(0, errors.Errors.Count);
    }
}