using System.Reflection;

namespace Nimble.StaticFiles.Templating;

internal static class EmbeddedHtmlTemplateLoader
{
    private const string TemplateName = "DirectoryListingTemplate.html";
    private const string ItemTemplateName = "ItemTemplate.html";
    
    internal static Task<string> LoadDirectoryListingTemplateAsync(CancellationToken cancellationToken = default) =>
        EmbeddedHtmlTemplateLoader.LoadTemplateAsync(
            TemplateName,
            cancellationToken);

    internal static Task<string> LoadDirectoryItemTemplateAsync(CancellationToken cancellationToken = default) =>
        EmbeddedHtmlTemplateLoader.LoadTemplateAsync(
            ItemTemplateName,
            cancellationToken);
    
    private static async Task<string> LoadTemplateAsync(
        string templateName,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(templateName);
        
        var assembly = Assembly
            .GetAssembly(typeof(EmbeddedHtmlTemplateLoader));
        
        if (assembly == null)
            throw new InvalidOperationException(
                $"Could not get the assembly for type '{nameof(EmbeddedHtmlTemplateLoader)}'. Embedded resources cannot be loaded.");
        
        var resourceNames = assembly.GetManifestResourceNames();

        var fullResourceName = resourceNames
            .FirstOrDefault(resource => resource.EndsWith(
                templateName,
                StringComparison.OrdinalIgnoreCase));
        
        if (fullResourceName == null)
            throw new FileNotFoundException($"Resource '{templateName}' not found in assembly '{assembly.FullName}'.");

        await using var stream = assembly.GetManifestResourceStream(fullResourceName);
        
        if (stream == null)
            throw new InvalidOperationException($"Resource stream for '{templateName}' not found.");

        using var reader = new StreamReader(stream);
        
        return await reader.ReadToEndAsync();
    }
}