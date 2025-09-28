using System.Net;
using Nimble.Extensions;
using Nimble.Http;

namespace Nimble.Controllers;

public abstract class Controller
{
    internal IReadOnlyDictionary<HttpVerb, HttpHandler> HttpMethodToHandlerMap { get; }

    protected Controller(Dictionary<HttpVerb, HttpHandler>? overrides = null)
    {
        var map = new Dictionary<HttpVerb, HttpHandler>
        {
            { HttpVerb.Post, PostAsync },
            { HttpVerb.Put, PutAsync },
            { HttpVerb.Get, GetAsync },
            { HttpVerb.Delete, DeleteAsync },
            { HttpVerb.Head, HeadAsync },
            { HttpVerb.Patch, PatchAsync },
            { HttpVerb.Options, OptionsAsync }
        };
        
        if (overrides != null)
        {
            foreach (var kv in overrides)
                map[kv.Key] = kv.Value;
        }
        
        HttpMethodToHandlerMap = map;
    }

    public virtual Task PostAsync(
        HttpListenerRequest request,
        HttpListenerResponse response,
        CancellationToken cancellationToken = default) => DefaultResponseAsync(response);

    public virtual Task PutAsync(
        HttpListenerRequest request,
        HttpListenerResponse response,
        CancellationToken cancellationToken = default) => DefaultResponseAsync(response);

    public virtual Task GetAsync(
        HttpListenerRequest request,
        HttpListenerResponse response,
        CancellationToken cancellationToken = default) => DefaultResponseAsync(response);

    public virtual Task DeleteAsync(
        HttpListenerRequest request,
        HttpListenerResponse response,
        CancellationToken cancellationToken = default) => DefaultResponseAsync(response);

    public virtual Task HeadAsync(
        HttpListenerRequest request,
        HttpListenerResponse response,
        CancellationToken cancellationToken = default) => DefaultResponseAsync(response);

    public virtual Task PatchAsync(
        HttpListenerRequest request,
        HttpListenerResponse response,
        CancellationToken cancellationToken = default) => DefaultResponseAsync(response);

    public virtual Task OptionsAsync(
        HttpListenerRequest request,
        HttpListenerResponse response,
        CancellationToken cancellationToken = default) => DefaultResponseAsync(response);

    public virtual Task TraceAsync(
        HttpListenerRequest request,
        HttpListenerResponse response,
        CancellationToken cancellationToken = default) => DefaultResponseAsync(response);

    public static Task DefaultResponseAsync(
        HttpListenerResponse response) =>
            response.RespondWithStatusCodeAsync(
                HttpStatusCode.NotImplemented);
}