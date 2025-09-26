namespace Nimble.Content;

public interface IFileExtensionToContentTypeMapper
{
    string GetContentTypeFor(string fileExtension);
}