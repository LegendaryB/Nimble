using System.Net;

namespace Nimble.Middleware;

public class RequestLoggingMiddleware : IMiddleware
{
    // private readonly 
    
    public Func<HttpListenerRequest, bool>? Filter { get; init; }
    
    public Task InvokeAsync(
        HttpListenerContext ctx,
        Func<CancellationToken, Task> next,
        CancellationToken cancellationToken = default)
    {
        var request = ctx.Request;
        
        if (Filter?.Invoke(request) ?? true)
        {
            
        }
    }
}