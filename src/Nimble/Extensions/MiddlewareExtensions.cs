using System.Net;
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
        
        var corsMiddleware = new CorsMiddleware(allowOrigin);

        return server.Use(corsMiddleware);
    }

    public static HttpServer UseRequestLogging(
        this HttpServer server,

        Func<HttpListenerRequest, bool>? filter = null)
    {
        ArgumentNullException.ThrowIfNull(server);
        
        return server;
    }
}