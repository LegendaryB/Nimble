using System.Net.Mime;

namespace Nimble.StaticFiles.Content;

internal class ContentTypeMapper : IContentTypeMapper
{
    public string GetContentTypeFor(string fileExtension)
    {
        return fileExtension switch
        {
            ".html" => MediaTypeNames.Text.Html,
            ".htm" => MediaTypeNames.Text.Html,
            ".css" => MediaTypeNames.Text.Css,
            ".js" => MediaTypeNames.Text.JavaScript,
            ".json" => MediaTypeNames.Application.Json,
            ".png" => MediaTypeNames.Image.Png,
            ".jpg" => MediaTypeNames.Image.Jpeg,
            ".jpeg" => MediaTypeNames.Image.Jpeg,
            ".gif" => MediaTypeNames.Image.Gif,
            _ => MediaTypeNames.Application.Octet
        };
    }
}