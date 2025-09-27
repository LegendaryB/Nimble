using System.Net;

namespace Nimble.Middleware;

public class CorsMiddleware(string allowOrigin = "*") : IMiddleware
{
    public async Task InvokeAsync(
        HttpListenerContext ctx,
        Func<CancellationToken, Task> next,
        CancellationToken cancellationToken = default)
    {
        ctx.Response.Headers.Add(
            HttpHeaderNames.AccessControlAllowOrigin
            , allowOrigin);
        
        if (ctx.Request.HttpMethod == "OPTIONS")
        {
            ctx.Response.StatusCode = (int)HttpStatusCode.NoContent;
            return;
        }

        await next(cancellationToken);
    }
}