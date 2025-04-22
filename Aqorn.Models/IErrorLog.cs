namespace Aqorn.Models;

public interface IErrorLog
{
    string Path { get; }
    string Current { get; }
    int ErrorCount { get; }

    void Add(string message);
    IErrorLog Step(string path);
}