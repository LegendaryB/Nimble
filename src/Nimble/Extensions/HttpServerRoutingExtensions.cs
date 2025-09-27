using Nimble.Controllers;

namespace Nimble.Extensions;

public static class HttpServerRoutingExtensions
{
    public static HttpServer MapPost(
        this HttpServer server,
        string route,
        HttpHandler handler) => server.Map(
            route,
            HttpVerb.Post,
            handler);
    
    public static HttpServer MapPut(
        this HttpServer server,
        string route,
        HttpHandler handler) => server.Map(
            route,
            HttpVerb.Put,
            handler);
    
    public static HttpServer MapGet(
        this HttpServer server,
        string route,
        HttpHandler handler) => server.Map(
            route,
            HttpVerb.Get,
            handler);
    
    public static HttpServer MapDelete(
        this HttpServer server,
        string route,
        HttpHandler handler) => server.Map(
            route,
            HttpVerb.Delete,
            handler);
    
    public static HttpServer MapHead(
        this HttpServer server,
        string route,
        HttpHandler handler) => server.Map(
            route,
            HttpVerb.Head,
            handler);
    
    public static HttpServer MapPatch(
        this HttpServer server,
        string route,
        HttpHandler handler) => server.Map(
            route,
            HttpVerb.Patch,
            handler);
    
    public static HttpServer MapOptions(
        this HttpServer server,
        string route,
        HttpHandler handler) => server.Map(
            route,
            HttpVerb.Options,
            handler);
    
    public static HttpServer MapTrace(
        this HttpServer server,
        string route,
        HttpHandler handler) => server.Map(
            route,
            HttpVerb.Trace,
            handler);

    private static HttpServer Map(
        this HttpServer server,
        string route,
        HttpVerb verb,
        HttpHandler handler)
    {
        ArgumentNullException.ThrowIfNull(server);
        ArgumentException.ThrowIfNullOrWhiteSpace(route);
        ArgumentNullException.ThrowIfNull(handler);

        return server.AddRoute(
            route,
            new DelegateController(handler, verb));
    }
}