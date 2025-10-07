namespace Nimble.Middleware;

public delegate Task NimbleMiddlewareDelegate(
    MiddlewareContext ctx,
    Func<CancellationToken, Task> next,
    CancellationToken cancellationToken);