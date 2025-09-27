using System.Net;

namespace Nimble;

public delegate Task HttpHandler(
    HttpListenerRequest request,
    HttpListenerResponse response,
    CancellationToken cancellationToken = default);