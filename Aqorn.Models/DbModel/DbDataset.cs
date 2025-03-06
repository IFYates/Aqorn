using Aqorn.Models.Data;
using Aqorn.Models.Spec;
using System.Diagnostics.CodeAnalysis;

namespace Aqorn.Models.DbModel;

public sealed class DbDataset
{
    public DbTable[] Tables { get; }

    public DbDataset(IErrorLog errors, ISpecSchema specModel)
    {
        Tables = specModel.Tables.Select(t => new DbTable(errors, this, t)).ToArray();
    }

    public bool TryGetTable(string name, [MaybeNullWhen(false)] out DbTable table)
    {
        table = Tables.FirstOrDefault(t => t.Name == name);
        return table != null;
    }

    public void Add(IErrorLog errors, IDataSchema dataModel)
    {
        foreach (var table in dataModel.Tables)
        {
            var tableErrors = errors.Step(table.Name);
            if (!TryGetTable(table.Name, out var tableData))
            {
                tableErrors.Add("Data table has no matching spec.");
            }
            else
            {
                tableData.AddData(tableErrors, table);
            }
        }
    }
}