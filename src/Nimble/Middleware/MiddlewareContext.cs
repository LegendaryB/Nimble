using System.Net;
using Nimble.Http;

namespace Nimble.Middleware;

public sealed class MiddlewareContext
{
    public HttpListenerRequest Request { get; init; }
    public HttpListenerResponse Response { get; init; }
    public HttpVerb RequestMethod { get; init; }

    internal MiddlewareContext(HttpListenerContext ctx)
    {
        Request = ctx.Request;
        Response = ctx.Response;
        
        RequestMethod = Enum.TryParse<HttpVerb>(
            Request.HttpMethod, 
            true,
            out var verb) ?
                verb :
                throw new InvalidOperationException($"Unsupported HTTP method: {Request.HttpMethod}");
    }
}