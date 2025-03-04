using Aqorn.Models.DbModel;
using Aqorn.Readers.Json.Data;
using Aqorn.Readers.Json.Spec;
using Aqorn.Writers.Mssql;

var specfile = @"F:\Neo\NEOne\DevOps\SeedData\Format.jsonc";
var datafile = @"F:\Neo\NEOne\DevOps\SeedData\Test.jsonc"; // TODO: multiple

// Parse files
var specModel = new JsonSpecFile(specfile);
var dataModel = new JsonDataFile(datafile);

if (specModel.Errors.Length > 0 || dataModel.Errors.Length > 0)
{
    Console.Error.WriteLine("[ERROR] Failed to parse files!");
    foreach (var err in specModel.Errors)
    {
        Console.Error.WriteLine("[SPEC] " + err);
    }
    foreach (var err in dataModel.Errors)
    {
        Console.Error.WriteLine("[DATA] " + err);
    }
    return;
}

// Build dataset
var dataset = new DbDataset(specModel);
dataset.Add(dataModel);

//if (dataset.Errors.Length > 0)
//{
//    Console.Error.WriteLine("[ERROR] Failed to generate output!");
//    foreach (var err in dataset.Errors)
//    {
//        Console.Error.WriteLine(err);
//    }
//    return;
//}

// Output
var writer = new ModelWriter(/* options? */);
var sql = writer.GenerateSql(dataset);
Console.WriteLine(sql);