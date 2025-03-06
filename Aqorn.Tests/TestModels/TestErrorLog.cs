using Aqorn.Models;

namespace Aqorn.Tests.TestModels;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal class TestErrorLog : IErrorLog
{
    public List<string> Errors { get; } = [];
    public int ErrorCount => Errors.Count;

    public string Path => string.Empty;

    public void Add(string message) => Errors.Add(message);

    public IErrorLog Step(string path) => this;
}
