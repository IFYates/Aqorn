using System.Diagnostics.CodeAnalysis;

namespace Aqorn.Models.Spec;

internal interface ISchemaSpec : ISpecModel
{
    TableSpec[] Tables { get; }

    bool TryGetTable(string name, [MaybeNullWhen(false)] out TableSpec tableSpec);
}