using Aqorn.Models.Data;
using Aqorn.Models.Spec;
using System.Diagnostics.CodeAnalysis;

namespace Aqorn.Models.DbModel;

internal class DbDataset : ModelBase, IModelValidator
{
    public DbTable[] Tables { get; }

    public DbDataset(ISchemaSpec specModel)
        : base(null!, null!)
    {
        Tables = specModel.Tables.Select(t => new DbTable(this, t)).ToArray();
    }

    public bool TryGetTable(string name, [MaybeNullWhen(false)] out DbTable table)
    {
        table = Tables.FirstOrDefault(t => t.Name == name);
        return table != null;
    }

    public void Add(IDataSchema dataModel)
    {
        foreach (var table in dataModel.Tables)
        {
            if (!TryGetTable(table.Name, out var tableData))
            {
                AddError(table, "Data table has no matching spec.");
            }
            else
            {
                tableData.AddData(table);
            }
        }
    }

    #region IModelValidator

    private readonly List<string> _errors = [];

    public string[] Errors => _errors.ToArray();

    public void AddError(IModel model, string text)
    {
        _errors.Add((model.Path.Length > 0 ? model.Path + ": " : null) + text);
    }

    #endregion IModelValidator
}