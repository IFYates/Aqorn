namespace Aqorn.Models;

public interface IErrorLog
{
    string Path { get; }

    void Add(string message);
    IErrorLog Step(string path);
}