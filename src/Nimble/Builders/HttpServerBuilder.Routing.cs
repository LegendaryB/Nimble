using Nimble.Controllers;
using Nimble.Http;
using Nimble.Routing;

namespace Nimble;

public partial class HttpServerBuilder
{
    public HttpServerBuilder MapPost(
        string route,
        HttpHandler handler) => Map(
            route,
            HttpVerb.Post,
            handler);
    
    public HttpServerBuilder MapPut(
        string route,
        HttpHandler handler) => Map(
            route,
            HttpVerb.Put,
            handler);
    
    public HttpServerBuilder MapGet(
        string route,
        HttpHandler handler) => Map(
            route,
            HttpVerb.Get,
            handler);
    
    public HttpServerBuilder MapDelete(
        string route,
        HttpHandler handler) => Map(
            route,
            HttpVerb.Delete,
            handler);
    
    public HttpServerBuilder MapHead(
        string route,
        HttpHandler handler) => Map(
            route,
            HttpVerb.Head,
            handler);
    
    public HttpServerBuilder MapPatch(
        string route,
        HttpHandler handler) => Map(
            route,
            HttpVerb.Patch,
            handler);
    
    public HttpServerBuilder MapOptions(
        string route,
        HttpHandler handler) => Map(
            route,
            HttpVerb.Options,
            handler);

    private HttpServerBuilder Map(
        string route,
        HttpVerb verb,
        HttpHandler handler)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(route);
        ArgumentNullException.ThrowIfNull(handler);

        return AddRoute(
            route,
            new DelegateController(handler, verb));
    }

    public HttpServerBuilder AddRoute<TController>(
        string route,
        bool isPrefix = false,
        string? parameterPattern = null)

        where TController : Controller, new() =>
            AddRoute(
                route,
                new TController(),
                isPrefix,
                parameterPattern);
    
    public HttpServerBuilder AddRoute<TController>(
        string route,
        TController controller,
        bool isPrefix = false,
        string? parameterPattern = null)
    
        where TController : Controller
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(route);
        ArgumentNullException.ThrowIfNull(controller);

        _server.Router.AddRoute(
            route,
            controller,
            isPrefix,
            parameterPattern);

        return this;
    }

    public HttpServerBuilder UseRouter<TRouter>()
        where TRouter : IHttpRouter, new() =>
            UseRouter(new TRouter());
    
    public HttpServerBuilder UseRouter<TRouter>(TRouter router)
        where TRouter : IHttpRouter
    {
        ArgumentNullException.ThrowIfNull(router);

        _server.Router = router;

        return this;
    }
}