using System.Diagnostics;
using Nimble.ConsoleApp.Settings;
using Nimble.Extensions;
using Spectre.Console.Cli;

namespace Nimble.ConsoleApp.Commands;

public class ServeStaticFilesCommand : ServeCommand<ServeStaticFilesCommandSettings>
{
    public override async Task<int> ExecuteAsync(
        CommandContext context,
        ServeStaticFilesCommandSettings settings)
    {
        try
        {
            var server = new HttpServer(settings.Prefix)
                .AddStaticRoute("/", settings.Path);

            Console.WriteLine($"Server is now running on {settings.Prefix}");
            OpenUrl(settings.Prefix);
            
            await server.RunAsync();
        
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);

            return 1;
        }
    }
}