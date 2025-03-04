using Aqorn.Models.Data;

namespace Aqorn.Models.DbModel;

internal class DbField : ModelBase
{
    public DbColumn Column { get; }
    public string? Value { get; }

    public DbField(DbColumn column, FieldModel data) : base(column, null!)
    {
        Column = column;

        Value = data.Value.ToString(); // TODO
    }
}