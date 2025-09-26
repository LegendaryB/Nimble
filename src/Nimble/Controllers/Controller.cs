using System.Net;
using Nimble.Extensions;

namespace Nimble.Controllers;

public abstract class Controller
{
    internal delegate Task HttpHandler(
        HttpListenerRequest request,
        HttpListenerResponse response,
        CancellationToken cancellationToken = default);

    internal IReadOnlyDictionary<HttpVerb, HttpHandler> HttpMethodToHandlerMap { get; }

    protected Controller()
    {
        HttpMethodToHandlerMap = new Dictionary<HttpVerb, HttpHandler>
        {
            { HttpVerb.Post, PostAsync },
            { HttpVerb.Put, PutAsync },
            { HttpVerb.Get, GetAsync },
            { HttpVerb.Delete, DeleteAsync },
            { HttpVerb.Head, HeadAsync },
            { HttpVerb.Patch, PatchAsync },
            { HttpVerb.Options, OptionsAsync },
            { HttpVerb.Trace, TraceAsync }
        };
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