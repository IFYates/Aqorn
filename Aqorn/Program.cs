using Aqorn.Models.DbModel;
using Aqorn.Readers;
using Aqorn.Readers.Json.Data;
using Aqorn.Readers.Json.Spec;
using Aqorn.Mssql.Writers;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Aqorn.Tests")]

var specfile = @"F:\Dev\Aqorn\Examples\ExampleSpec.jsonc";
var datafile = @"F:\Dev\Aqorn\Examples\ExampleData.jsonc"; // TODO: multiple

// Parse files
var errorLog = new SourceErrorLog();
var specModel = new JsonSpecFileReader(errorLog.Step("spec"), specfile);
var dataModel = new JsonDataFileReader(errorLog.Step("data"), datafile);

if (errorLog.ErrorCount > 0)
{
    Console.Error.WriteLine("[ERROR] Failed to parse files!");
    foreach (var err in errorLog.Errors)
    {
        Console.Error.WriteLine($"{err.Path}: {err.Message}");
    }
    return;
}

// Build dataset
var dataset = new DbDataset(errorLog, specModel);
dataset.Add(errorLog, dataModel);

if (errorLog.ErrorCount > 0)
{
    Console.Error.WriteLine("[ERROR] Failed to generate output!");
    foreach (var err in errorLog.Errors)
    {
        Console.Error.WriteLine($"{err.Path}: {err.Message}");
    }
    return;
}

// Output
var writer = new DatasetWriter(/* options? */);
var sql = writer.GenerateSql(dataset);
Console.WriteLine(sql);