using Nimble.Controllers;
using Nimble.Routing;

namespace Nimble.Extensions;

public static class HttpServerExtensions
{
    public static IHttpServer AddRoute<TController>(
        this IHttpServer server,
        string route,
        bool isPrefix = false,
        string? parameterPattern = null)

        where TController : Controller, new() =>
            server.AddRoute(
                route,
                new TController(),
                isPrefix,
                parameterPattern);
    
    public static IHttpServer AddRoute<TController>(
        this IHttpServer server,
        string route,
        TController controller,
        bool isPrefix = false,
        string? parameterPattern = null)
    
        where TController : Controller
    {
        ArgumentNullException.ThrowIfNull(server);
        ArgumentException.ThrowIfNullOrWhiteSpace(route);
        ArgumentNullException.ThrowIfNull(controller);

        server.Router.AddRoute(
            route,
            controller,
            isPrefix,
            parameterPattern);
        
        return server;
    }
}