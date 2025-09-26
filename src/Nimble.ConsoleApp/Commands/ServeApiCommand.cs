using Nimble.ConsoleApp.Controllers;
using Nimble.ConsoleApp.Settings;
using Nimble.Extensions;
using Spectre.Console.Cli;

namespace Nimble.ConsoleApp.Commands;

public class ServeApiCommand : ServeCommand<ServeApiCommandSettings>
{
    private const string ApiPath = "api/persons";
    
    public override async Task<int> ExecuteAsync(
        CommandContext context,
        ServeApiCommandSettings settings)
    {
        try
        {
            var server = new HttpServer(settings.Prefix)
                .AddRoute<PersonController>($"/{ApiPath}");

            var apiEndpoint = $"{settings.Prefix.TrimEnd('/')}/{ApiPath}";
            
            Console.WriteLine($"Server is now running on {settings.Prefix}");
            Console.WriteLine($"API Endpoint is reachable via: {apiEndpoint}");
            OpenUrl(apiEndpoint);
            
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