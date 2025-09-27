using System.Net;

namespace Nimble.Middleware;

public interface IMiddleware
{
    Task InvokeAsync(
        HttpListenerContext ctx,
        Func<CancellationToken, Task> next,
        CancellationToken cancellationToken = default);
}