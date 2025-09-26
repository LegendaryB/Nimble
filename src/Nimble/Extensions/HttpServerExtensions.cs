using Nimble.Controllers;
using Nimble.Routing;

namespace Nimble.Extensions;

public static class HttpServerExtensions
{
    public static HttpServer AddRoute<TController>(
        this HttpServer server,
        string route,
        bool isPrefix = false,
        string? parameterPattern = null)

        where TController : Controller, new() =>
            server.AddRoute(
                route,
                new TController(),
                isPrefix,
                parameterPattern);
    
    public static HttpServer AddRoute<TController>(
        this HttpServer server,
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

    public static HttpServer UseRouter<TRouter>(
        this HttpServer server)

        where TRouter : IRouter, new() => server.UseRouter(new TRouter());
    
    public static HttpServer UseRouter<TRouter>(
        this HttpServer server,
        TRouter router)

        where TRouter : IRouter
    {
        ArgumentNullException.ThrowIfNull(server);
        ArgumentNullException.ThrowIfNull(router);
        
        server.Router = router;
        
        return server;
    }
}