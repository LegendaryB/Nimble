using Nimble.Content;
using Nimble.Controllers;

namespace Nimble.Extensions;

public static class HttpServerExtensions
{
    public static HttpServer AddStaticRoute(
        this HttpServer server,
        string route,
        string folderPath,
        IFileExtensionToContentTypeMapper? fileExtensionToContentTypeMapper = null)
    {
        ArgumentNullException.ThrowIfNull(server);
        ArgumentException.ThrowIfNullOrWhiteSpace(route);
        ArgumentException.ThrowIfNullOrWhiteSpace(folderPath);
        
        if (!Directory.Exists(folderPath))
            throw new DirectoryNotFoundException($"Folder '{folderPath}' not found.");

        fileExtensionToContentTypeMapper ??= new FileExtensionToContentTypeMapper();

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