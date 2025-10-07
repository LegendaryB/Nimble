using System.Net;
using Nimble.Http;

namespace Nimble.Middleware;

internal class CorsMiddleware(string allowOrigin = "*") : IMiddleware
{
    public async Task InvokeAsync(
        MiddlewareContext ctx,
        Func<CancellationToken, Task> next,
        CancellationToken cancellationToken = default)
    {
        ctx.Response.Headers.Add(
            HttpHeaderNames.AccessControlAllowOrigin
            , allowOrigin);
        
        if (ctx.RequestMethod == HttpVerb.Options)
        {
            ctx.Response.StatusCode = (int)HttpStatusCode.NoContent;
            return;
        }

        await next(cancellationToken);
    }
}