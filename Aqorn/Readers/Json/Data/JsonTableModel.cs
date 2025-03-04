using Aqorn.Models;
using Aqorn.Models.Data;
using System.Text.Json;

namespace Aqorn.Readers.Json.Data;

internal class JsonTableModel : TableModel
{
    public JsonTableModel(IModel parent, string name, JsonElement json)
        : base(parent, name)
    {
        if (json.ValueKind == JsonValueKind.Array)
        {
            var rows = new List<TableRowModel>();
            var idx = 0;
            foreach (var row in json.EnumerateArray())
            {
                if (row.ValueKind != JsonValueKind.Object)
                {
                    Error($"Table row data must be an object ('{Name}[{idx++}]' gave {row.ValueKind}).");
                    continue;
                }
                rows.Add(new JsonTableRowModel(this, idx++.ToString(), row));
            }
            Rows = rows.ToArray();
        }
        else if (json.ValueKind == JsonValueKind.Object)
        {
            Rows = [new JsonTableRowModel(this, null!, json)];
        }
        else
        {
            Error($"Table must be an object ('{Name}' gave {json.ValueKind}).");
        }
    }
}