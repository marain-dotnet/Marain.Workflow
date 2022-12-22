// See https://aka.ms/new-console-template for more information
using Marain.Workflows;
using System;
using Microsoft.Extensions.Logging;
public class MinimalConsoleLogger : ILogger<LogAction>
{
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        Console.WriteLine($"{logLevel}: [{eventId}] {formatter(state, exception)}");
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public IDisposable BeginScope<TState>(TState state) => throw new NotImplementedException();
}


