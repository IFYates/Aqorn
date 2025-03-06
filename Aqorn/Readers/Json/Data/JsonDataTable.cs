using Aqorn.Models;
using Aqorn.Models.Data;
using System.Text.Json;

namespace Aqorn.Readers.Json.Data;

public sealed class JsonDataTable : IDataTable
{
    public string Name { get; }
    public IDataSchema Schema { get; }
    public IDataRow? ParentRow { get; }
    public IDataRow[] Rows { get; }

    public JsonDataTable(IErrorLog errors, string name, JsonElement json, IDataSchema schema)
        : this(errors, name, json, schema, null)
    {
    }
    public JsonDataTable(IErrorLog errors, string name, JsonElement json, IDataRow parent)
        : this(errors, name, json, parent.Table.Schema, parent)
    {
    }
    private JsonDataTable(IErrorLog errors, string name, JsonElement json, IDataSchema schema, IDataRow? parent)
    {
        Name = name;
        Schema = schema;
        ParentRow = parent;
        Rows = parse(errors.Step(name), json);
    }

    private IDataRow[] parse(IErrorLog errors, JsonElement json)
    {
        switch (json.ValueKind)
        {
            case JsonValueKind.Array:
                var rows = new List<IDataRow>();
                var idx = 0;
                foreach (var row in json.EnumerateArray())
                {
                    var rerrs = errors.Step(idx++.ToString());
                    if (row.ValueKind != JsonValueKind.Object)
                    {
                        errors.Add($"Table row data must be an object (was {row.ValueKind}).");
                        continue;
                    }
                    rows.Add(new JsonDataRow(rerrs, this, row));
                }
                return rows.ToArray();
            case JsonValueKind.Object:
                return [new JsonDataRow(errors, this, json)];

            default:
                errors.Add($"Table must be an object (was {json.ValueKind}).");
                return [];
        }
    }
}