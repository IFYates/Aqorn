using Aqorn.Models.Spec;

namespace Aqorn.Models.Data;

internal class TableRowModel(ModelBase parent, string name)
    : ModelBase(parent, name)
{
    public FieldModel[] Fields { get; protected init; } = [];
    public TableModel[] Relationships { get; protected init; } = [];

    public TableSpec? Spec { get; private set; }
    public void Validate(TableSpec tableSpec)
    {
        Spec = tableSpec;

        // Check fields
        foreach (var field in Fields)
        {
            if (tableSpec.TryGetField(field.Name, out var fieldSpec))
            {
                field.Validate(fieldSpec);
            }
            else
            {
                Error("Unable to locate spec for field.");
            }
        }

        // Check for missing fields
        foreach (var fieldSpec in tableSpec.Fields)
        {
            if (fieldSpec.ValueType?.IsRequired == true
                && !TryGetField(fieldSpec.Name, out _))
            {
                Error($"Missing field in data ({fieldSpec.Name}).");
            }
        }
    }
}