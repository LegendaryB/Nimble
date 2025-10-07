using System.Net;
using Nimble.Http;

namespace Nimble.Middleware;

internal class BlockTraceMiddleware : IMiddleware
{
    public async Task InvokeAsync(
        MiddlewareContext ctx,
        Func<CancellationToken, Task> next,
        CancellationToken cancellationToken = default)
    {
        if (ctx.RequestMethod == HttpVerb.Trace)
        {
            ctx.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
            return;
        }

        await next(cancellationToken);
    }
}