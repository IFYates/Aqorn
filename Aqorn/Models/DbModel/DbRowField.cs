using Aqorn.Models.Spec;
using Aqorn.Models.Values;
using Aqorn.Readers;

namespace Aqorn.Models.DbModel;

internal class DbRowField(DbRowData row, DbColumn column, IValue value)
{
    public DbRowData Row { get; } = row;
    public DbColumn Column { get; } = column;
    public string Name => Column.Name;
    public IValue? Value { get; private set; }

    public bool IsParameter => Name[0] == '@';
    public bool IncludeInInsert => !IsParameter && (Value != null || Column.IsRequired || Column.DefaultValue != null);

    private DbColumn[] findDependencies(IErrorLog errors, IValue value)
    {
        switch (value)
        {
            case FieldValue fv:
                var table = Column.Table;
                switch (fv.Type)
                {
                    case FieldValue.ValueType.Self:
                        if (!table.TryGetColumn(fv.Value, out var selfField))
                        {
                            errors.Add("Could not find self field.");
                            return [];
                        }
                        return [selfField];
                    case FieldValue.ValueType.Parent:
                        if (table.Parent == null)
                        {
                            errors.Add("No parent table to resolve.");
                            return [];
                        }
                        if (!table.Parent.TryGetColumn(fv.Value, out var parentField))
                        {
                            errors.Add($"Could not find parent field ({fv.Value}).");
                            return [];
                        }
                        return [parentField];
                    case FieldValue.ValueType.Parameter:
                        if (!table.TryGetColumn("@" + fv.Value, out var parameter))
                        {
                            errors.Add("Could not find parameter.");
                            return [];
                        }
                        return [parameter];
                    default:
                        return [];
                }
            case ConcatenatedValue cv:
                if (Column.Type is not FieldValue.ValueType.String and not FieldValue.ValueType.Regex)
                {
                    errors.Add($"Concatenated value can not replace {Column.Type}.");
                    return [];
                }
                return cv.Values.SelectMany(v => findDependencies(errors, v)).ToArray();
            case QueryValueSpec qv:
                if (Column.Type is not FieldValue.ValueType.String and not FieldValue.ValueType.Regex)
                {
                    errors.Add($"Subquery can not replace {Column.Type}.");
                    return [];
                }
                return qv.Fields.SelectMany(f => findDependencies(errors, f.Value)).ToArray();
            default:
                errors.Add("Invalid field value.");
                return [];
        }
    }

    public bool ResolveValue(IErrorLog errors)
    {
        if (Value != null)
        {
            return true;
        }
        errors = errors.Step(Column.Name);

        var depends = findDependencies(errors, value);
        if (depends.Length > 0)
        {
            foreach (var depend in depends)
            {
                var drow = depend.Table == Column.Table.Parent
                    ? Row.ParentRow
                    : Row;
                if (drow?.TryGetField(depend.Name, out var dependField) is null or false)
                {
                    errors.Add("Missing is required.");
                    return true;
                }
                if (dependField.Value == null)
                {
                    return false;
                }
            }
        }

        Value = getFieldValue(value, Row);
        if (Value == null)
        {
            if (Column.IsRequired)
            {
                errors.Add("Value is required.");
            }
        }
        // TODO
        //else if (Column.ValueType?.Length < Value.Length)
        //{
        //    errors.Add("Value is too long.");
        //}
        //else if (Column.Type == FieldValue.ValueType.Regex
        //    && !Column.ValueType!.Regex!.IsMatch(Value))
        //{
        //    errors.Add("Value does not match regex.");
        //}
        return true;
    }

    private static IValue? getFieldValue(IValue value, DbRowData row)
    {
        return value switch
        {
            ConcatenatedValue cv
                => FieldValue.String(string.Join("", cv.Values.Select(v => getFieldValue(v, row)))),
            QueryValueSpec qv 
                => qv, // TODO: resolve to subquery
            FieldValue fv => fv.Type switch
            {
                FieldValue.ValueType.Null => FieldValue.Null,
                FieldValue.ValueType.Parameter => row.TryGetField("@" + fv.Value, out var parameter) && parameter.Value != null
                    ? parameter.Value : null,
                FieldValue.ValueType.Parent => row.ParentRow?.TryGetField(fv.Value, out var parField) == true && parField.Value != null
                    ? parField.Value : null,
                FieldValue.ValueType.Self => row.TryGetField(fv.Value, out var selfField) && selfField.Value != null
                    ? selfField.Value : null,
                _ => fv
            },
            _ => null
        };
    }
}