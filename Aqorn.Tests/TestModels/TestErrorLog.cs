using Aqorn.Models;

namespace Aqorn.Tests.TestModels;

internal class TestErrorLog : IErrorLog
{
    public List<string> Errors { get; } = [];

    public string Path => string.Empty;

    public void Add(string message) => Errors.Add(message);

    public IErrorLog Step(string path) => this;
}
