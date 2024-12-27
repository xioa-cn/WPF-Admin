using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Xioa.Admin.Service.Web.Api.Controllers;

/// <summary>
/// 文件上传控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private readonly ILogger<FileController> _logger;
    private readonly IConfiguration _configuration;
    
    // 允许的文件类型
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf", ".doc", ".docx", ".xls", ".xlsx" };
    // 最大文件大小 (10MB)
    private const int MaxFileSize = 10 * 1024 * 1024;

    public FileController(ILogger<FileController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// 单文件上传
    /// </summary>
    /// <param name="file">文件</param>
    /// <param name="description">文件描述</param>
    /// <returns>上传结果</returns>
    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(
        [Required] IFormFile files,
        string? description = null)
    {
        try
        {
            // 验证文件
            var validationResult = ValidateFile(files);
            if (validationResult != null)
            {
                return validationResult;
            }

            // 生成文件名
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(files.FileName)}";
            
            // 获取上传路径
            var uploadPath = _configuration["FileStorage:UploadPath"] 
                ?? Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            
            // 确保目录存在
            Directory.CreateDirectory(uploadPath);
            
            // 完整文件路径
            var filePath = Path.Combine(uploadPath, fileName);

            // 保存文件
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await files.CopyToAsync(stream);
            }

            // 返回结果
            return Ok(new
            {
                fileName = fileName,
                originalName = files.FileName,
                size = files.Length,
                path = filePath,
                description
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "File upload failed");
            return StatusCode(500, "File upload failed");
        }
    }

    /// <summary>
    /// 多文件上传
    /// </summary>
    /// <param name="files">文件列表</param>
    /// <returns>上传结果</returns>
    [HttpPost("upload/multiple")]
    public async Task<IActionResult> UploadMultipleFiles([Required] List<IFormFile> files)
    {
        try
        {
            var results = new List<object>();

            foreach (var file in files)
            {
                // 验证文件
                var validationResult = ValidateFile(file);
                if (validationResult != null)
                {
                    continue;
                }

                // 生成文件名
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                
                // 获取上传路径
                var uploadPath = _configuration["FileStorage:UploadPath"] 
                    ?? Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
                
                // 确保目录存在
                Directory.CreateDirectory(uploadPath);
                
                // 完整文件路径
                var filePath = Path.Combine(uploadPath, fileName);

                // 保存文件
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // 添加结果
                results.Add(new
                {
                    fileName = fileName,
                    originalName = file.FileName,
                    size = file.Length,
                    path = filePath
                });
            }

            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Multiple files upload failed");
            return StatusCode(500, "Multiple files upload failed");
        }
    }

    /// <summary>
    /// 分块上传
    /// </summary>
    [HttpPost("upload/chunk")]
    public async Task<IActionResult> UploadChunk(
        [Required] IFormFile file,
        [Required] int chunkNumber,
        [Required] int totalChunks,
        [Required] string fileId)
    {
        try
        {
            // 获取临时目录
            var tempPath = Path.Combine(
                _configuration["FileStorage:TempPath"] 
                    ?? Path.Combine(Directory.GetCurrentDirectory(), "Temp"),
                fileId);

            // 确保目录存在
            Directory.CreateDirectory(tempPath);

            // 保存分块
            var chunkPath = Path.Combine(tempPath, $"chunk_{chunkNumber}");
            using (var stream = new FileStream(chunkPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 检查是否所有分块都已上传
            if (chunkNumber == totalChunks)
            {
                // 合并文件
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var uploadPath = _configuration["FileStorage:UploadPath"] 
                    ?? Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
                Directory.CreateDirectory(uploadPath);
                var finalPath = Path.Combine(uploadPath, fileName);

                await MergeChunksAsync(tempPath, finalPath, totalChunks);

                // 清理临时文件
                Directory.Delete(tempPath, true);

                return Ok(new
                {
                    fileName,
                    path = finalPath,
                    size = new FileInfo(finalPath).Length
                });
            }

            return Ok(new { chunkNumber, status = "Chunk uploaded successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Chunk upload failed");
            return StatusCode(500, "Chunk upload failed");
        }
    }

    /// <summary>
    /// 验证文件
    /// </summary>
    private IActionResult? ValidateFile(IFormFile file)
    {
        // 检查文件大小
        if (file.Length > MaxFileSize)
        {
            return BadRequest($"File size exceeds maximum limit of {MaxFileSize / 1024 / 1024}MB");
        }

        // 检查文件类型
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
        {
            return BadRequest($"File type {extension} is not allowed");
        }

        return null;
    }

    /// <summary>
    /// 合并文件分块
    /// </summary>
    private async Task MergeChunksAsync(string tempPath, string targetPath, int totalChunks)
    {
        using var targetStream = new FileStream(targetPath, FileMode.Create);
        
        for (int i = 1; i <= totalChunks; i++)
        {
            var chunkPath = Path.Combine(tempPath, $"chunk_{i}");
            using var sourceStream = new FileStream(chunkPath, FileMode.Open);
            await sourceStream.CopyToAsync(targetStream);
        }
    }
}