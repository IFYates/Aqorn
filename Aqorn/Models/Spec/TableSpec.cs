using System.Diagnostics.CodeAnalysis;

namespace Aqorn.Models.Spec;

internal class TableSpec(ModelBase parent, string name)
    : ModelBase(parent, name), ISpecModel
{
    private readonly string _tableName = null!;
    public string? SchemaName { get; protected init; }
    public string TableName
    {
        get => _tableName;
        protected init
        {
            var period = value.IndexOf('.');
            if (period >= 0)
            {
                SchemaName = value[..period];
                _tableName = value[(period + 1)..];
            }
            else
            {
                SchemaName = null;
                _tableName = value;
            }
        }
    }

    public bool IdentityInsert { get; protected init; }
    public FieldSpec[] Fields { get; protected init; } = [];
    public TableSpec[] Relationships { get; protected init; } = [];

    public bool TryGetField(string name, [MaybeNullWhen(false)] out FieldSpec spec)
    {
        spec = Fields.FirstOrDefault(f => f.Name == name);
        return spec != null;
    }
}