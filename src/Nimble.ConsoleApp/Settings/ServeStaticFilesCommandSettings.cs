using Spectre.Console.Cli;

namespace Nimble.ConsoleApp.Settings;

public class ServeStaticFilesCommandSettings : ServeCommandSettings
{
    [CommandOption("-p|--path <Path>")]
    public required string Path { get; set; }
}