using System.Net;
using Nimble.Controllers;

namespace Nimble.Routing;

public interface IHttpRouter
{
    Task RouteAsync(
        HttpListenerRequest request,
        HttpListenerResponse response,
        CancellationToken cancellationToken = default);

    void AddRoute<TController>(
        string route,
        TController controller,
        bool isPrefix = false,
        string? parameterPattern = null)

        where TController : Controller;
}