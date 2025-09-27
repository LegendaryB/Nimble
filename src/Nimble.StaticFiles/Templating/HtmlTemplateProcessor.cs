namespace Nimble.StaticFiles.Templating;

internal static class HtmlTemplateProcessor
{
    private const string PlaceholderPrefix = "{{";
    private const string PlaceholderSuffix = "}}";
    
    internal static string Replace(
        string template,
        IDictionary<string, string> replacements)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(template);
        ArgumentNullException.ThrowIfNull(replacements);

        return replacements.Aggregate(template, (current, kvp) => Replace(current, kvp.Key, kvp.Value));
    }
    
    private static string Replace(
        string template,
        string placeholder,
        string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(template);
        ArgumentException.ThrowIfNullOrWhiteSpace(placeholder);
        
        placeholder = NormalizePlaceholder(placeholder);
        
        return template.Replace(
            placeholder,
            value);
    }

    private static string NormalizePlaceholder(string placeholder)
    {
        if (!placeholder.StartsWith(PlaceholderPrefix))
            placeholder = PlaceholderPrefix + placeholder;
        
        if (!placeholder.EndsWith(PlaceholderSuffix))
            placeholder += PlaceholderSuffix;
        
        return placeholder;
    }
}