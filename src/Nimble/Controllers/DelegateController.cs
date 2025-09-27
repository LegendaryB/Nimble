using System.Net;

namespace Nimble.Controllers;

internal class DelegateController(
    HttpHandler handler,
    HttpVerb verb = HttpVerb.Get)
    
    : Controller(new Dictionary<HttpVerb, HttpHandler>
    {
        [verb] = handler.Invoke
    });