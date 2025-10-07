using System.Net;

namespace Nimble.Middleware;

public interface IMiddleware
{
    Task InvokeAsync(
        MiddlewareContext ctx,
        Func<CancellationToken, Task> next,
        CancellationToken cancellationToken = default);
}