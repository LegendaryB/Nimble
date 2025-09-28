using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Nimble.Http;

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
    
    public static Task RespondWithProblemDetailsAsync(
        this HttpListenerResponse response,
        string title,
        string? detail = null,
        string? type = null,
        string? instance = null,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest,
        IDictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(response);
        ArgumentNullException.ThrowIfNull(title);
        
        var problem = new ProblemDetails
        {
            Type = type!,
            Title = title,
            Status = (int)statusCode,
            Detail = detail,
            Instance = instance
        };

        return response.RespondWithProblemDetailsAsync(
            problem,
            statusCode,
            headers,
            cancellationToken);
    }

    public static Task RespondWithProblemDetailsAsync(
        this HttpListenerResponse response,
        ProblemDetails problem,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest,
        IDictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(response);
        ArgumentNullException.ThrowIfNull(problem);

        problem = SetProblemDetailsDefaults(problem);
        
        return response.RespondWithJsonAsync(
            problem,
            statusCode,
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
    
    private static ProblemDetails SetProblemDetailsDefaults(
        ProblemDetails problem,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        ArgumentNullException.ThrowIfNull(problem);

        problem.Status ??= (int)statusCode;
        problem.Type ??= "about:blank";
        
        if (string.IsNullOrWhiteSpace(problem.Title))
            problem.Title = ((HttpStatusCode)problem.Status).ToString();

        return problem;
    }
}