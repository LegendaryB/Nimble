using Spectre.Console.Cli;

namespace Nimble.ConsoleApp.Settings;

public abstract class ServeCommandSettings : CommandSettings
{
    [CommandArgument(0, "<Prefix>")]
    public required string Prefix { get; set; }
}