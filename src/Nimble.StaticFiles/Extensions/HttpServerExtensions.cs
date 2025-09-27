using Nimble.Extensions;
using Nimble.StaticFiles.Content;
using Nimble.StaticFiles.Controllers;

namespace Nimble.StaticFiles.Extensions;

public static class HttpServerExtensions
{
    public static HttpServer AddStaticRoute(
        this HttpServer server,
        string route,
        string folderPath,
        IContentTypeMapper? fileExtensionToContentTypeMapper = null)
    {
        ArgumentNullException.ThrowIfNull(server);
        ArgumentException.ThrowIfNullOrWhiteSpace(route);
        ArgumentException.ThrowIfNullOrWhiteSpace(folderPath);
        
        if (!Directory.Exists(folderPath))
            throw new DirectoryNotFoundException($"Folder '{folderPath}' not found.");

        fileExtensionToContentTypeMapper ??= new ContentTypeMapper();

        return server
            .AddRoute(
                route,
                new StaticFileController(
                    route,
                    folderPath,
                    fileExtensionToContentTypeMapper),
                isPrefix: true);
    }
}