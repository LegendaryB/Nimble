using System.Net;
using System.Text;
using Nimble.Templating.Constants;

namespace Nimble.Templating;

internal class DirectoryListingGenerator
{
    private const string DirectoryUnicodeIcon = "\ud83d\udcc1";
    private const string FileUnicodeIcon = "\ud83d\udcc4";

    private const string SelfTarget = "_self";
    private const string BlankTarget = "_blank";

    private const string ToParentFolderDisplayName = "..";
    
    private readonly string _rootFolderPath;

    internal DirectoryListingGenerator(string rootFolderPath)
    {
        _rootFolderPath = rootFolderPath;
    }

    internal async Task<string> GenerateHtmlAsync(
        string directoryPath,
        HttpListenerRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directoryPath);
        ArgumentNullException.ThrowIfNull(request);

        var template = await EmbeddedHtmlTemplateLoader.LoadDirectoryListingTemplateAsync(cancellationToken);
        var itemTemplate = await EmbeddedHtmlTemplateLoader.LoadDirectoryItemTemplateAsync(cancellationToken);

        var itemsHtmlBuilder = new StringBuilder();

        var parentFolderItem = GenerateParentFolderItemIfNecessary(
            directoryPath,
            itemTemplate,
            request);
        
        if (!string.IsNullOrWhiteSpace(parentFolderItem))
            itemsHtmlBuilder.AppendLine(parentFolderItem);

        var itemsHtmlList = GenerateFilesystemItems(
            directoryPath,
            itemTemplate,
            request);
        
        foreach (var itemsHtml in itemsHtmlList)
            itemsHtmlBuilder.AppendLine(itemsHtml);
        
        return HtmlTemplateProcessor.Replace(
            template, 
            new Dictionary<string, string>
        {
            [DirectoryListingTemplatePlaceholders.Path] = Uri.UnescapeDataString(request.Url?.AbsolutePath!),
            [DirectoryListingTemplatePlaceholders.Items] = itemsHtmlBuilder.ToString()
        });
    }

    private string? GenerateParentFolderItemIfNecessary(
        string directoryPath,
        string itemTemplate,
        HttpListenerRequest request)
    {
        if (string.Equals(
            directoryPath,
            _rootFolderPath,
            StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }
        
        var parentDir = Directory.GetParent(directoryPath);

        if (parentDir == null ||
            !parentDir.FullName.StartsWith(
                _rootFolderPath,
                StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }
        
        var href = request.Url!.AbsolutePath.TrimEnd('/');
        var lastSlash = href.LastIndexOf('/');
        href = lastSlash > 0 ? href[..lastSlash] : "/";

        return CreateItemHtml(
            itemTemplate, 
            ToParentFolderDisplayName, 
            href,
            true);
    }

    private List<string> GenerateFilesystemItems(
        string directoryPath,
        string itemTemplate,
        HttpListenerRequest request)
    {
        var filesystemItems = Directory
            .GetFileSystemEntries(directoryPath)
            .OrderByDescending(path => File.GetAttributes(path).HasFlag(FileAttributes.Directory));

        var items = new List<string>();
        
        foreach (var filesystemItem in filesystemItems)
        {
            var displayName = Path.GetFileName(filesystemItem);
            var href = $"{request.Url?.AbsolutePath.TrimEnd('/')}/{Uri.EscapeDataString(displayName)}/";
            var isDirectory = File.GetAttributes(filesystemItem).HasFlag(FileAttributes.Directory);

            var itemHtml = CreateItemHtml(
                itemTemplate,
                displayName,
                href,
                isDirectory);
                
            items.Add(itemHtml);
        }

        return items;
    }

    private static string CreateItemHtml(
        string template,
        string displayName,
        string href,
        bool isDirectory = false)
    {
        return HtmlTemplateProcessor.Replace(
            template,
            CreateItemTemplateMapping(
                displayName,
                GetItemIcon(isDirectory),
                href,
                GetItemTarget(isDirectory)));
    }
    
    private static IDictionary<string, string> CreateItemTemplateMapping(
        string displayName,
        string icon,
        string href,
        string target)
    {
        return new Dictionary<string, string>
        {
            [ItemTemplatePlaceholders.HrefAttribute] = href,
            [ItemTemplatePlaceholders.TargetAttribute] = target,
            [ItemTemplatePlaceholders.ContentDisplayName] = displayName,
            [ItemTemplatePlaceholders.ContentIcon] = icon,
        };
    }
    
    private static string GetItemIcon(bool isDirectory) => isDirectory ? $"{DirectoryUnicodeIcon}" : $"{FileUnicodeIcon}";
    private static string GetItemTarget(bool isDirectory) => isDirectory ? SelfTarget : BlankTarget;
}