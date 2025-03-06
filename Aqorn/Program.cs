using Aqorn;
using Aqorn.Models.DbModel;
using Aqorn.Mssql.Writers;
using Aqorn.Readers;
using Aqorn.Readers.Json.Data;
using Aqorn.Readers.Json.Spec;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Aqorn.Tests")]

// Arguments
var errorLog = new SourceErrorLog();
var options = new Options(errorLog, new(args));
if (errorLog.ErrorCount > 0)
{
    Console.Error.WriteLine("Invalid arguments.");
    foreach (var err in errorLog.Errors)
    {
        Console.Error.WriteLine(err.Message);
    }
    return;
}

// Parse files
var specModel = new JsonSpecFileReader(errorLog.Step("spec"), options.SpecFile);
var dataModels = options.DataFiles.Select((f, i) =>
{
    var name = "data" + (options.DataFiles.Count > 1 ? i : null);
    var dataModel = new JsonDataFileReader(errorLog.Step(name), f);
    foreach (var (key, value) in options.Parameters)
    {
        dataModel.SetParameter(key, value);
    }
    return dataModel;
}).ToArray();

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
foreach (var dataModel in dataModels)
{
    dataset.Add(errorLog, dataModel);
}

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
var writer = new DatasetWriter(options);
var sql = writer.GenerateSql(dataset);
Console.WriteLine(sql);