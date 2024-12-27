namespace Xioa.Admin.Request.Tools.Model;

public class FileUploadContent
{
    public Stream FileStream { get; }
    public string FileName { get; }
    public string? ContentType { get; }

    public FileUploadContent(Stream fileStream, string fileName, string? contentType = null)
    {
        FileStream = fileStream;
        FileName = fileName;
        ContentType = contentType;
    }
}