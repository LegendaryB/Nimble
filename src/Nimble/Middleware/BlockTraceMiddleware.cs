using System.Net;

namespace Nimble.Middleware;

public class BlockTraceMiddleware : IMiddleware
{
    public async Task InvokeAsync(
        HttpListenerContext ctx,
        Func<CancellationToken, Task> next,
        CancellationToken cancellationToken = default)
    {
        if (ctx.Request.HttpMethod == "TRACE")
        {
            ctx.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
            return;
        }

        await next(cancellationToken);
    }
}