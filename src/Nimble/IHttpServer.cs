using Nimble.Middleware;
using Nimble.Routing;

namespace Nimble;

public interface IHttpServer
{
    IHttpRouter Router { get; }
    
    Task RunAsync(CancellationToken cancellationToken = default);

    public IHttpServer Use(NimbleMiddlewareDelegate middleware);

    public IHttpServer Use<TMiddleware>()
        where TMiddleware : IMiddleware, new();

    public IHttpServer Use<TMiddleware>(TMiddleware middleware)
        where TMiddleware : IMiddleware;
}