namespace Nimble.ConsoleApp.Contracts;

public record PersonContract
{
    public required string Name { get; init; }
    
    public required int Age { get; init; }
    
    public required string Gender { get; init; }
}