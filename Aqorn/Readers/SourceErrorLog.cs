using Aqorn.Models;

namespace Aqorn.Readers;

/// <summary>
/// Provides a central way to log errors during processing.
/// </summary>
public sealed class SourceErrorLog : IErrorLog
{
    private readonly IList<(string Path, string Message)> _errors;
    public int ErrorCount => _errors.Count;
    public (string Path, string Message)[] Errors => _errors.ToArray();

    public string Path { get; }
    public string Current { get; }

    public SourceErrorLog()
    {
        _errors = [];
        Path = string.Empty;
        Current = string.Empty;
    }
    private SourceErrorLog(IList<(string Path, string Message)> errors, string path, string name)
    {
        _errors = errors;
        if (name.Contains('.'))
        {
            name = $"[{name}]";
        }
        Path = path.Length > 0 ? $"{path}.{name}" : name;
        Current = name;
    }

    public IErrorLog Step(string name)
        => new SourceErrorLog(_errors, Path, name);
    public void Add(string message)
        => _errors.Add((Path, message));
}