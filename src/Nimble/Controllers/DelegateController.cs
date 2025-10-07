using Nimble.Http;

namespace Nimble.Controllers;

internal class DelegateController(
    HttpHandler handler,
    HttpVerb requestMethod = HttpVerb.Get)
    
    : Controller(new Dictionary<HttpVerb, HttpHandler>
    {
        [requestMethod] = handler.Invoke
    });