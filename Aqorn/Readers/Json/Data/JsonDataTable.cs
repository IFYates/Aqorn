using Aqorn.Models.Data;
using System.Text.Json;

namespace Aqorn.Readers.Json.Data;

internal sealed class JsonDataTable : IDataTable
{
    public string Name { get; }
    public IDataRow[] Rows { get; }

    public JsonDataTable(IErrorLog errors, string name, JsonElement json)
    {
        Name = name;
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
                    if (row.ValueKind != JsonValueKind.Object)
                    {
                        errors.Add($"Table row data must be an object ('{Name}[{idx++}]' gave {row.ValueKind}).");
                        continue;
                    }
                    rows.Add(new JsonDataRow(errors.Step(idx++.ToString()), this, row));
                }
                return rows.ToArray();
            case JsonValueKind.Object:
                return [new JsonDataRow(errors, this, json)];

            default:
                errors.Add($"Table must be an object ('{Name}' gave {json.ValueKind}).");
                return [];
        }
    }
}