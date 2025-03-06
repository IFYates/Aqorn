using Aqorn.Models.Values;

namespace Aqorn.Models.DbModel;

public sealed class DbValue
{
    private readonly DbColumn[] _dependencies;
    private readonly List<string> _depErrors = [];
    private readonly DbDataRow _row;

    public IValue Source { get; }
    public IValue? Value { get; private set; }

    public DbValue(IValue source, DbDataRow row)
    {
        Source = source;
        _row = row;
        _dependencies = findDependencies(source, row);
    }

    private DbColumn[] findDependencies(IValue value, DbDataRow row)
    {
        switch (value)
        {
            case FieldValue fv:
                var table = row.Table;
                switch (fv.Type)
                {
                    case FieldValue.ValueType.Self:
                        if (!table.TryGetColumn(fv.Value, out var selfField))
                        {
                            _depErrors.Add($"Could not find self field ({fv.Value}).");
                            return [];
                        }
                        return [selfField];
                    case FieldValue.ValueType.Parent:
                        if (table.Parent == null)
                        {
                            _depErrors.Add("No parent table to resolve.");
                            return [];
                        }
                        if (!table.Parent.TryGetColumn(fv.Value, out var parentField))
                        {
                            _depErrors.Add($"Could not find parent field ({fv.Value}).");
                            return [];
                        }
                        return [parentField];
                    case FieldValue.ValueType.Parameter:
                        if (!table.TryGetColumn(fv.Value, out var parameter))
                        {
                            _depErrors.Add($"Could not find parameter ({fv.Value}).");
                            return [];
                        }
                        return [parameter];
                    default:
                        return [];
                }
            case ConcatenatedValue cv:
                return cv.Values.SelectMany(v => findDependencies(v, row)).ToArray();
            case SubqueryValue qv:
                return qv.Fields.SelectMany(f => findDependencies(f.Value, row)).ToArray();
            default:
                _depErrors.Add($"Invalid field value ({value.Type}).");
                return [];
        }
    }

    /// <summary>
    /// Resolves complex values to something writable.
    /// Returns true if there are no unresolved dependency values.
    /// </summary>
    public bool TryResolveValue(IErrorLog errors)
    {
        if (_depErrors.Count > 0)
        {
            _depErrors.ForEach(errors.Add);
            return true;
        }

        if (Value == null)
        {
            foreach (var depend in _dependencies)
            {
                var drow = depend.Table == _row.Table
                    ? _row
                    : _row.ParentRow;
                if (drow?.TryGetField(depend.Name, out var dependField) is null or false)
                {
                    errors.Add($"Unable to resolve reference ({depend.Name}).");
                    return true;
                }
                if (dependField.Value == null)
                {
                    return false;
                }
            }

            Value = Source.Resolve(_row);
        }

        return true;
    }
}