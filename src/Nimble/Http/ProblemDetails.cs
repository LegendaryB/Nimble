namespace Nimble.Http;

public class ProblemDetails
{
    public required string Type { get; set; }
    
    public required string Title { get; set; }
    
    public int? Status { get; set; }
    
    public string? Detail { get; set; }
    
    public string? Instance { get; set; }
    
    public IDictionary<string, object?> Extensions { get; set; } = new Dictionary<string, object?>();
}