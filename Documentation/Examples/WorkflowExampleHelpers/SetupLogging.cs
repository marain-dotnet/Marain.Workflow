// See https://aka.ms/new-console-template for more information
using Marain.Workflows;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

#nullable disable annotations
public class SetupLogging
{
    public ILogger<LogAction> Logger { get; }
    public SetupLogging()
    {
        ServiceCollection services = new();
        //services.AddLogging();
        services.AddLogging(logging =>
        {
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Debug);
        });
        IServiceProvider serviceProvider = services.BuildServiceProvider();
        this.Logger = serviceProvider.GetRequiredService<ILogger<LogAction>>();
    }
}


