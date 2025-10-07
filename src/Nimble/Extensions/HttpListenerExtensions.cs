using System.Net;

namespace Nimble.Extensions;

public static class HttpListenerExtensions
{
    internal static async Task<(bool Success, HttpListenerContext? Context)> TryGetContextAsync(
        this HttpListener httpListener)
    {
        ArgumentNullException.ThrowIfNull(httpListener);

        try
        {
            var ctx = await httpListener.GetContextAsync();

            return (true, ctx);
        }
        catch (Exception ex)
        {
            return (false, null);
        }
    }
}