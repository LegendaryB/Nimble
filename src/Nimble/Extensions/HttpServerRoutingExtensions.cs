using Nimble.Controllers;
using Nimble.Http;

namespace Nimble.Extensions;

public static class HttpServerRoutingExtensions
{
    public static IHttpServer MapPost(
        this IHttpServer server,
        string route,
        HttpHandler handler) => server.Map(
            route,
            HttpVerb.Post,
            handler);
    
    public static IHttpServer MapPut(
        this IHttpServer server,
        string route,
        HttpHandler handler) => server.Map(
            route,
            HttpVerb.Put,
            handler);
    
    public static IHttpServer MapGet(
        this IHttpServer server,
        string route,
        HttpHandler handler) => server.Map(
            route,
            HttpVerb.Get,
            handler);
    
    public static IHttpServer MapDelete(
        this IHttpServer server,
        string route,
        HttpHandler handler) => server.Map(
            route,
            HttpVerb.Delete,
            handler);
    
    public static IHttpServer MapHead(
        this IHttpServer server,
        string route,
        HttpHandler handler) => server.Map(
            route,
            HttpVerb.Head,
            handler);
    
    public static IHttpServer MapPatch(
        this IHttpServer server,
        string route,
        HttpHandler handler) => server.Map(
            route,
            HttpVerb.Patch,
            handler);
    
    public static IHttpServer MapOptions(
        this HttpServer server,
        string route,
        HttpHandler handler) => server.Map(
            route,
            HttpVerb.Options,
            handler);

    private static IHttpServer Map(
        this IHttpServer server,
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