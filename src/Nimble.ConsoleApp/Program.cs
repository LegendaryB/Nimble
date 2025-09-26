using Nimble.ConsoleApp.Commands;
using Spectre.Console.Cli;

namespace Nimble.ConsoleApp;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        var app = new CommandApp();
        
        app.Configure(configuration =>
        {
            configuration.AddCommand<ServeApiCommand>("serve-api");
            configuration.AddCommand<ServeStaticFilesCommand>("serve-static");
        });

        await app.RunAsync(args);
    }
}