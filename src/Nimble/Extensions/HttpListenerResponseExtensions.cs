using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace Nimble.Extensions;

public static class HttpListenerResponseExtensions
{
    public static Task RespondWithStatusCodeAsync(
        this HttpListenerResponse response,
        HttpStatusCode statusCode = HttpStatusCode.OK,
        IDictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(response);
        
        return response.WriteResponseAsync(
            statusCode,
            headers: headers,
            cancellationToken: cancellationToken);
    }

    public static Task RespondWithStatusCodeAsync(
        this HttpListenerResponse response,
        string body,
        HttpStatusCode statusCode = HttpStatusCode.OK,
        string? contentType = MediaTypeNames.Text.Plain,
        IDictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(response);
        ArgumentNullException.ThrowIfNull(body);
        
        var bytes = Encoding.UTF8.GetBytes(body);
        
        return response.RespondWithStatusCodeAsync(
            bytes,
            statusCode,
            contentType,
            headers,
            cancellationToken);
    }
    
    public static Task RespondWithStatusCodeAsync(
        this HttpListenerResponse response,
        byte[] body,
        HttpStatusCode statusCode = HttpStatusCode.OK,
        string? contentType = MediaTypeNames.Application.Octet,
        IDictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(response);
        ArgumentNullException.ThrowIfNull(body);
        
        return response.WriteResponseAsync(
            statusCode,
            body,
            contentType,
            headers,
            cancellationToken);
    }

    public static Task RespondWithJsonAsync<T>(
        this HttpListenerResponse response,
        T body,
        HttpStatusCode statusCode = HttpStatusCode.OK,
        IDictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(response);
        ArgumentNullException.ThrowIfNull(body);

        var json = JsonSerializer.Serialize(body);
        var bytes = Encoding.UTF8.GetBytes(json);

        return response.RespondWithStatusCodeAsync(
            bytes,
            statusCode,
            MediaTypeNames.Application.Json,
            headers,
            cancellationToken);
    }

    private static async Task WriteResponseAsync(
        this HttpListenerResponse response,
        HttpStatusCode statusCode = HttpStatusCode.OK,
        byte[]? body = null,
        string? contentType = null,
        IDictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(response);
        
        response.StatusCode = (int)statusCode;

        if (headers is not null)
        {
            foreach (var (key, value) in headers)
                response.Headers[key] = value;
        }

        if (body is { Length: > 0 } &&
            statusCode != HttpStatusCode.NoContent &&
            statusCode != HttpStatusCode.NotModified)
        {
            response.ContentType = contentType ?? MediaTypeNames.Text.Plain;
            response.ContentLength64 = body.Length;
            
            await response.OutputStream.WriteAsync(
                body,
                cancellationToken);
        }
        else
        {
            response.ContentLength64 = 0;
        }
    }
}