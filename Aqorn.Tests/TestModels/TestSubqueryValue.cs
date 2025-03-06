using Aqorn.Models.Data;
using Aqorn.Models.Values;

namespace Aqorn.Tests.TestModels;

public class TestSubqueryValue : SubqueryValue
{
    public TestSubqueryValue(string? schemaName, string tableName, string fieldName,
        IDataField[] fields)
    {
        SchemaName = schemaName;
        TableName = tableName;
        FieldName = fieldName;
        FieldsSpec = fields;
    }
}