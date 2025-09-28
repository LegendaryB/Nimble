using System.Net;
using System.Text.Json;

namespace Nimble.Extensions;

public static class HttpListenerRequestExtensions
{
    public static async Task<T?> ReadFromJsonAsync<T>(
        this HttpListenerRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var reader = new StreamReader(
            request.InputStream,
            request.ContentEncoding);

        var body = await reader.ReadToEndAsync(cancellationToken);

        return JsonSerializer.Deserialize<T>(body);
    }
}