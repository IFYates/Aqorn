using System.Diagnostics.CodeAnalysis;

namespace Aqorn.Models;

/// <summary>
/// Validates values against a definition.
/// </summary>
internal abstract class ValueBase(IModel parent)
    : ModelBase(parent, null!)
{
    public virtual bool TryResolve(/*model,*/ [MaybeNullWhen(false)] out string value)
    {
        // TODO
        value = null;
        return false;
    }
}