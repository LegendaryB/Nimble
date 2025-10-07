using System.Net;
using Nimble.Extensions;

namespace Nimble;

public partial class HttpServerBuilder
{
    private readonly HttpServer _server;

    internal HttpServerBuilder(
        IEnumerable<string> prefixes,
        bool allowHttpTrace = false)
    {
        _server = new HttpServer(prefixes);

        if (!allowHttpTrace)
            _server.UseRequestBlocking();
    }
    
    public static HttpServerBuilder Create(bool allowHttpTrace = false)
    {
        return Create(
            "http://localhost:5000",
            allowHttpTrace);
    }
    
    public static HttpServerBuilder Create(
        string prefix,
        bool allowHttpTrace = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
        
        return Create(
            allowHttpTrace,
            prefix);
    }
    
    public static HttpServerBuilder Create(
        bool allowHttpTrace = false,
        params string[] prefixes)
    {
        if (prefixes.Length == 0)
        {
            throw new ArgumentException(
                "At least one prefix must be provided.",
                nameof(prefixes));
        }
        
        foreach (var prefix in prefixes)
        {
            if (string.IsNullOrWhiteSpace(prefix))
            {
                throw new ArgumentException(
                    "Prefix cannot be null or whitespace.",
                    nameof(prefixes));
            }

            if (!Uri.TryCreate(prefix, UriKind.Absolute, out _))
            {
                throw new ArgumentException(
                    $"Invalid prefix: {prefix}",
                    nameof(prefixes));
            }
        }

        return new HttpServerBuilder(
            prefixes.ToArray(),
            allowHttpTrace);
    }

    public HttpServerBuilder WithRequestReceivedCallback(Func<HttpListenerRequest, Task> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);

        _server.SetOnRequestReceivedCallback(callback);
        
        return this;
    }
    
    public HttpServerBuilder WithResponseSentCallback(Func<HttpListenerResponse, Task> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);

        _server.SetOnResponseSentCallback(callback);
        
        return this;
    }
    
    public HttpServerBuilder WithUnhandledExceptionCallback(Func<Exception, HttpListenerContext, Task> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        
        _server.SetOnUnhandledExceptionCallback(callback);
        
        return this;
    }
    
    public HttpServerBuilder WithServerStartedCallback(Func<Task> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        
        _server.SetOnServerStartedCallback(callback);
        
        return this;
    }
    
    public HttpServerBuilder WithServerStoppedCallback(Func<Task> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        
        _server.SetOnServerStoppedCallback(callback);
        
        return this;
    }
    
    public IHttpServer Build() => _server;
}