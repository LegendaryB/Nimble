using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace Nimble.Extensions;

public static class HttpListenerResponseExtensions
{
    public static async Task RespondWithStatusCodeAsync(
        this HttpListenerResponse response,
        string body,
        HttpStatusCode statusCode,
        string? contentType = MediaTypeNames.Text.Plain,
        IDictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default)
    {
        var bytes = Encoding.UTF8.GetBytes(body);
        
        await response.RespondWithStatusCodeAsync(
            bytes,
            statusCode,
            contentType,
            headers,
            cancellationToken);
    }

    public static Task RespondWithStatusCodeAsync(
        this HttpListenerResponse response,
        HttpStatusCode statusCode)
    {
        response.StatusCode = (int)statusCode;
        response.ContentLength64 = 0;
        
        return Task.CompletedTask;
    }

    public static async Task RespondWithStatusCodeAsync(
        this HttpListenerResponse response,
        byte[] body,
        HttpStatusCode statusCode,
        string? contentType = null,
        IDictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default)
    {
        response.StatusCode = (int)statusCode;
        response.ContentType = contentType ?? MediaTypeNames.Text.Plain;

        if (headers is not null)
        {
            foreach (var (key, value) in headers)
                response.Headers[key] = value;
        }

        if (body.Length > 0 &&
            statusCode != HttpStatusCode.NoContent &&
            statusCode != HttpStatusCode.NotModified)
        {
            response.ContentLength64 = body.Length;
            await response.OutputStream.WriteAsync(body, cancellationToken);
        }
        else
        {
            response.ContentLength64 = 0;
        }
    }

    public static Task RespondWithJsonAsync<T>(
        this HttpListenerResponse response,
        T body,
        HttpStatusCode statusCode = HttpStatusCode.OK,
        IDictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(body);
        var bytes = Encoding.UTF8.GetBytes(json);
        
        return response.RespondWithStatusCodeAsync(
            bytes,
            statusCode,
            MediaTypeNames.Application.Json,
            headers,
            cancellationToken);
    }
}