namespace Xioa.Admin.Request.Tools.Model;

/// <summary>
/// 文件上传内容模型
/// </summary>
public class FileUploadContent
{
    /// <summary>
    /// 文件流
    /// </summary>
    public Stream FileStream { get; set; } = null!;

    /// <summary>
    /// 文件名
    /// </summary>
    public string FileName { get; set; } = null!;

    /// <summary>
    /// 内容类型（MIME类型）
    /// 如果为空，将根据文件扩展名自动推断
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// 文件大小（字节）
    /// </summary>
    public long FileSize => FileStream?.Length ?? 0;
}