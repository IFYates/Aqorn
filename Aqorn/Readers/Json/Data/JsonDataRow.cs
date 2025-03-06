using Aqorn.Models;
using Aqorn.Models.Data;
using System.Text.Json;

namespace Aqorn.Readers.Json.Data;

public sealed class JsonDataRow : IDataRow
{
    public IDataTable Table { get; }
    public IDataField[] Fields { get; }
    public IDataTable[] Relationships { get; }

    public JsonDataRow(IErrorLog errors, IDataTable table, JsonElement json)
    {
        Table = table;

        var properties = json.EnumerateObject().ToArray();

        Fields = properties.Where(p => p.Name[0] != ':')
            .Select(f => new JsonDataField(errors, f.Name, f.Value))
            .ToArray();

        Relationships = properties.Where(p => p.Name[0] == ':')
            .Select(r => new JsonDataTable(errors, r.Name[1..], r.Value, this))
            .ToArray();
    }
}