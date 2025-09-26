using System.Diagnostics;
using Spectre.Console.Cli;

namespace Nimble.ConsoleApp.Commands;

public abstract class ServeCommand<TCommandSettings> : AsyncCommand<TCommandSettings>
    where TCommandSettings : CommandSettings
{
    protected void OpenUrl(string url)
    {
        var psi = new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true,
        };
            
        Process.Start(psi);
    }
}