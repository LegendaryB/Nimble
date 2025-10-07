using System.Net;
using Nimble.Extensions;
using Nimble.Middleware;

namespace Nimble;

public partial class HttpServerBuilder
{
    public HttpServerBuilder UseCors(string allowOrigin = "*")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(allowOrigin);
        
        _server.UseCors(allowOrigin);

        return this;
    }

    public HttpServerBuilder UseRequestBlocking(
        Func<MiddlewareContext, bool>? predicate = null,
        HttpStatusCode? statusCode = null)
    {
        _server.UseRequestBlocking(
            predicate,
            statusCode);

        return this;
    }

    public HttpServerBuilder UseRequestLogging(
        Func<MiddlewareContext, CancellationToken, Task> loggerDelegate,
        Func<HttpListenerRequest, bool>? predicate = null)
    {
        ArgumentNullException.ThrowIfNull(loggerDelegate);

        _server.UseRequestLogging(
            loggerDelegate,
            predicate);

        return this;
    }
}