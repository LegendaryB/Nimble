using System.Net;

namespace Nimble.Middleware;

internal class BlockRequestMiddleware : IMiddleware
{
    private readonly Func<MiddlewareContext, bool> _predicate;
    private readonly int _statusCode;
    
    internal BlockRequestMiddleware(
        Func<MiddlewareContext, bool> predicate,
        HttpStatusCode statusCode)
    {
        _predicate = predicate;
        _statusCode = (int)statusCode;
    }
    
    public async Task InvokeAsync(
        MiddlewareContext ctx,
        Func<CancellationToken, Task> next,
        CancellationToken cancellationToken = default)
    {
        if (_predicate(ctx))
        {
            ctx.Response.StatusCode = _statusCode;
            return;
        }

        await next(cancellationToken);
    }
}