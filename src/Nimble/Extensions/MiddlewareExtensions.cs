using System.Net;
using Nimble.Http;
using Nimble.Middleware;

namespace Nimble.Extensions;

public static class MiddlewareExtensions
{
    public static HttpServer UseCors(
        this HttpServer server,
        string allowOrigin = "*")
    {
        ArgumentNullException.ThrowIfNull(server);
        ArgumentException.ThrowIfNullOrWhiteSpace(allowOrigin);
        
        var middleware = new CorsMiddleware(allowOrigin);

        return server.Use(middleware);
    }

    public static HttpServer UseRequestBlocking(
        this HttpServer server,
        Func<MiddlewareContext, bool>? predicate = null,
        HttpStatusCode? statusCode = null)
    {
        statusCode ??= (predicate is null ? HttpStatusCode.MethodNotAllowed : HttpStatusCode.Forbidden);
        predicate ??= ctx => ctx.RequestMethod == HttpVerb.Trace;
        
        var middleware = new BlockRequestMiddleware(
            predicate,
            statusCode.Value);

        return server.Use(middleware);
    }

    public static HttpServer UseRequestLogging(
        this HttpServer server,
        Func<MiddlewareContext, CancellationToken, Task> loggerDelegate,
        Func<HttpListenerRequest, bool>? predicate = null)
    {
        ArgumentNullException.ThrowIfNull(server);
        ArgumentNullException.ThrowIfNull(loggerDelegate);
        
        predicate ??= _ => true;
        
        var middleware = new RequestLoggingMiddleware(
            loggerDelegate,
            predicate);

        return server.Use(middleware);
    }
}