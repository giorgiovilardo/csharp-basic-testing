using System;
using System.IO;
using Xunit;

namespace MyApp.ConsoleRunner.Tests;

public class ConsoleRunnerTests
{
    [Fact]
    public void WritesHelloReaderToConsole()
    {
        var capturedOutput = new StringWriter();
        Console.SetOut(capturedOutput);
        Program.Main(Array.Empty<string>());
        Assert.Contains("Hello, reader!", capturedOutput.ToString());
    }
}
