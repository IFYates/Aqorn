namespace Aqorn.Readers;

internal class SourceErrorLog : IErrorLog
{
    private readonly List<(string Path, string Message)> _errors;
    public int ErrorCount => _errors.Count;
    public (string Path, string Message)[] Errors => _errors.ToArray();

    public string Path { get; }

    public SourceErrorLog()
    {
        _errors = [];
        Path = string.Empty;
    }
    private SourceErrorLog(SourceErrorLog parent, string path)
    {
        _errors = parent._errors;
        if (path.Contains('.'))
        {
            path = $"[{path}]";
        }
        Path = parent.Path.Length > 0 ? $"{parent.Path}.{path}" : path;
    }

    public SourceErrorLog Step(string path) => new(this, path);

    public void Add(string message)
    {
        _errors.Add((Path, message));
    }
}