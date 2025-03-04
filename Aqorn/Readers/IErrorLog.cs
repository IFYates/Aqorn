namespace Aqorn.Readers;

internal interface IErrorLog
{
    string Path { get; }

    void Add(string message);
    SourceErrorLog Step(string path);
}