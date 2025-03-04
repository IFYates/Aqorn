using Aqorn.Models.Data;
using System.Text.Json;

namespace Aqorn.Readers.Json.Data;

internal class JsonTableModel : TableModel
{
    public JsonTableModel(IErrorLog errors, string name, JsonElement json)
        : base(name)
    {
        Rows = parse(errors, json);
    }

    private TableRowModel[] parse(IErrorLog errors, JsonElement json)
    {
        switch (json.ValueKind)
        {
            case JsonValueKind.Array:
                {
                    var rows = new List<TableRowModel>();
                    var idx = 0;
                    foreach (var row in json.EnumerateArray())
                    {
                        if (row.ValueKind != JsonValueKind.Object)
                        {
                            errors.Add($"Table row data must be an object ('{Name}[{idx++}]' gave {row.ValueKind}).");
                            continue;
                        }
                        rows.Add(new JsonTableRowModel(errors.Step(idx++.ToString()), this, row));
                    }
                    return rows.ToArray();
                }

            case JsonValueKind.Object:
                return [new JsonTableRowModel(errors, this, json)];

            default:
                errors.Add($"Table must be an object ('{Name}' gave {json.ValueKind}).");
                return [];
        }
    }
}