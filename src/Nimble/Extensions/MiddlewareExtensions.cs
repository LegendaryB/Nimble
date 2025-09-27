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
}