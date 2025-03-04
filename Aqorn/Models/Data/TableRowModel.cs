using Aqorn.Models.Spec;
using System.Diagnostics.CodeAnalysis;

namespace Aqorn.Models.Data;

internal class TableRowModel(IModel parent, string name)
    : ModelBase(parent, name), IDataModel<TableSpec>
{
    public FieldModel[] Fields { get; protected init; } = [];
    public TableModel[] Relationships { get; protected init; } = [];

    public bool TryGetField(string name, [MaybeNullWhen(false)] out FieldModel field)
    {
        field = Fields.FirstOrDefault(f => f.Name == name);
        return field != null;
    }

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