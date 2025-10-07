using System.Net;
using System.Text.RegularExpressions;
using Nimble.Controllers;
using Nimble.Extensions;
using Nimble.Http;

namespace Nimble.Routing;

internal sealed class HttpRouter : IHttpRouter
{
    private readonly List<RouteEntry> _routes = [];

    public Task RouteAsync(
        HttpListenerRequest request,
        HttpListenerResponse response,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (request.Url == null)
                return Controller.DefaultResponseAsync(response);

            var path = request.Url.AbsolutePath;

            RouteEntry? match = null;

            foreach (var route in _routes)
            {
                if (route.IsPrefix &&
                    path.StartsWith(
                        route.RoutePattern,
                        StringComparison.OrdinalIgnoreCase))
                {
                    match = route;
                    break;
                }

                if (route.ParameterRegex != null &&
                    route.ParameterRegex.IsMatch(path))
                {
                    match = route;
                    break;
                }

                if (route.IsPrefix ||
                    !string.Equals(
                        route.RoutePattern,
                        path,
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                match = route;
                break;
            }

            if (match == null)
                return Controller.DefaultResponseAsync(response);

            if (!Enum.TryParse<HttpVerb>(
                    request.HttpMethod,
                    true,
                    out var method))
            {
                return Controller.DefaultResponseAsync(response);
            }

            if (!match.Controller.HttpMethodToHandlerMap.TryGetValue(
                    method,
                    out var httpMethodHandler))
            {
                return Controller.DefaultResponseAsync(response);
            }

            try
            {
                return httpMethodHandler.Invoke(
                    request,
                    response,
                    cancellationToken);
            }
            catch
            {
                return response.RespondWithStatusCodeAsync(
                    HttpStatusCode.InternalServerError);
            }
        }
        catch (Exception ex)
        {
            return Controller.DefaultResponseAsync(response);
        }
    }

    public void AddRoute<TController>(
        string route,
        TController controller,
        bool isPrefix = false,
        string? parameterPattern = null)
        
        where TController : Controller
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(route);
        ArgumentNullException.ThrowIfNull(controller);
        
        Regex? regex = null;

        if (!string.IsNullOrEmpty(parameterPattern))
        {
            regex = new Regex(
                parameterPattern,
                RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        var routeEntry = new RouteEntry(
            route,
            controller,
            isPrefix,
            regex);

        _routes.Add(routeEntry);
    }
}