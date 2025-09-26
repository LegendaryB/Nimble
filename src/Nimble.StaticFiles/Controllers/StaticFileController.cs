using System.Net;
using System.Net.Mime;
using Nimble.Content;
using Nimble.Extensions;
using Nimble.Templating;

namespace Nimble.Controllers;

internal class StaticFileController : Controller
{
    private readonly string _route;
    private readonly string _rootFolderPath;
    private readonly IFileExtensionToContentTypeMapper _contentTypeMapper;
    private readonly DirectoryListingGenerator _directoryListingGenerator;

    internal StaticFileController(
        string route,
        string rootFolderPath,
        IFileExtensionToContentTypeMapper contentTypeMapper)
    {
        _route = route.TrimEnd('/');

        _rootFolderPath = Path.TrimEndingDirectorySeparator(Path.IsPathRooted(rootFolderPath)
            ? Path.GetFullPath(rootFolderPath)
            : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, rootFolderPath)));

        _contentTypeMapper = contentTypeMapper;

        _directoryListingGenerator = new DirectoryListingGenerator(_rootFolderPath);
    }

    public override async Task GetAsync(
        HttpListenerRequest request,
        HttpListenerResponse response,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(response);
        
        if (request.Url == null)
        {
            await DefaultResponseAsync(response);
            return;
        }

        var path = ResolvePath(request);

        if (!IsPathAllowed(path))
        {
            await response.RespondWithStatusCodeAsync(
                "Forbidden",
                HttpStatusCode.Forbidden);

            return;
        }

        if (File.Exists(path))
        {
            await ServeFileAsync(
                path,
                response,
                cancellationToken);

            return;
        }

        if (Directory.Exists(path))
        {
            await ServeDirectoryAsync(
                path,
                request,
                response,
                cancellationToken);

            return;
        }

        await response.RespondWithStatusCodeAsync(
            "Not found",
            HttpStatusCode.NotFound);
    }

    

    private async Task ServeFileAsync(
        string path,
        HttpListenerResponse response,
        CancellationToken cancellationToken = default)
    {
        var extension = Path
            .GetExtension(path)
            .ToLowerInvariant();
        
        var contentType = _contentTypeMapper.GetContentTypeFor(extension);

        var bytes = await File.ReadAllBytesAsync(
            path,
            cancellationToken);
        
        await response.RespondWithStatusCodeAsync(
            bytes,
            HttpStatusCode.OK,
            contentType,
            cancellationToken: cancellationToken);
    }

    private async Task ServeDirectoryAsync(
        string path,
        HttpListenerRequest request,
        HttpListenerResponse response,
        CancellationToken cancellationToken = default)
    {
        var defaultFileNames = new[] { "index.html", "index.htm" };

        var indexFile = defaultFileNames
            .Select(file => Path.Combine(path, file))
            .FirstOrDefault(File.Exists);
        
        if (indexFile != null)
        {
            await ServeFileAsync(
                indexFile,
                response,
                cancellationToken);
            
            return;
        }

        var html = await _directoryListingGenerator.GenerateHtmlAsync(
            path,
            request,
            cancellationToken);
        
        await response.RespondWithStatusCodeAsync(
            html,
            HttpStatusCode.OK,
            MediaTypeNames.Text.Html);
    }
    
    private string ResolvePath(HttpListenerRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        var relativePath = request.Url!.AbsolutePath[_route.Length..]
            .TrimStart('/')
            .Replace('/', Path.DirectorySeparatorChar);

        relativePath = Uri.UnescapeDataString(relativePath);

        var path = Path.Combine(
            _rootFolderPath,
            relativePath);
        
        return Path.TrimEndingDirectorySeparator(Path.GetFullPath(path));
    }
    
    private bool IsPathAllowed(string path) => path.StartsWith(
        _rootFolderPath,
        StringComparison.OrdinalIgnoreCase);
}