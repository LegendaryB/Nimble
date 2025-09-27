using System.Net;

namespace Nimble.Middleware;

public delegate Task NimbleMiddlewareDelegate(
    HttpListenerContext context,
    Func<CancellationToken, Task> next,
    CancellationToken cancellationToken);