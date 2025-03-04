using Aqorn.Readers.Json.Data;
using Aqorn.Readers.Json.Spec;
using Aqorn.Writers.Mssql;

var specfile = @"F:\Neo\NEOne\DevOps\SeedData\Format.jsonc";
var datafile = @"F:\Neo\NEOne\DevOps\SeedData\Test.jsonc"; // TODO: multiple

// Spec
var specModel = new JsonSpecFile(specfile);
var specErrors = specModel.Validate();

// Data
var dataModel = new JsonDataFile(datafile);
var dataErrors = dataModel.Validate(specModel);

if (specErrors.Length > 0 || dataErrors.Length > 0)
{
    foreach (var err in specErrors)
    {
        Console.Error.WriteLine("[SPEC] " + err);
    }
    foreach (var err in dataErrors)
    {
        Console.Error.WriteLine("[DATA] " + err);
    }
    Console.Error.WriteLine("[INVALID] Processing cannot continue!");
    return;
}

// Output
var writer = new ModelWriter(/* options? */);
var sql = writer.GenerateSql(dataModel, specModel);
Console.WriteLine(sql);