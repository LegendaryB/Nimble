namespace Nimble.StaticFiles.Content;

public interface IContentTypeMapper
{
    string GetContentTypeFor(string fileExtension);
}