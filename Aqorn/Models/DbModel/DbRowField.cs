using Aqorn.Models.Values;
using Aqorn.Readers;

namespace Aqorn.Models.DbModel;

internal sealed class DbRowField(DbDataRow row, DbColumn column, IValue source)
{
    public DbDataRow Row { get; } = row;
    public DbColumn Column { get; } = column;
    public string Name => Column.Name;
    public DbValue SourceValue { get; } = new DbValue(source, row);
    public IValue? Value { get; private set; }

    public bool IsParameter { get; } = column.Name[0] == '@';
    public bool IncludeInInsert => !IsParameter && (SourceValue.Source != FieldValue.Null || Column.IsRequired || Column.DefaultValue != null);

    public bool TryApplyValue(IErrorLog errors)
    {
        if (!SourceValue.TryResolveValue(errors))
        {
            return false;
        }

        Value = SourceValue.Value;
        switch (Value)
        {
            case null:
                if (Column.IsRequired)
                {
                    errors.Add("Value is required.");
                }
                break;
            case FieldValue fv when fv != FieldValue.Null:
                Value = Column.Validate(fv, errors);
                break;
        }
        return true;
    }
}