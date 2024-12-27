using System.Net;

namespace Xioa.Admin.Request.Tools.Model;

public class ProgressStreamContent : StreamContent {
    private readonly Stream _stream;
    private readonly IProgress<double> _progress;
    private readonly int _bufferSize;
    private bool _disposed;

    /// <summary>
    /// 初始化进度流内容处理器
    /// </summary>
    /// <param name="stream">要上传的流</param>
    /// <param name="bufferSize">缓冲区大小</param>
    /// <param name="progress">进度报告接口</param>
    public ProgressStreamContent(
        Stream stream,
        int bufferSize,
        IProgress<double> progress) : base(stream, bufferSize) {
        _stream = stream;
        _progress = progress;
        _bufferSize = bufferSize;
    }

    /// <summary>
    /// 重写序列化方法，添加进度报告
    /// </summary>
    protected override async Task SerializeToStreamAsync(
        Stream stream,
        TransportContext? context,
        CancellationToken cancellationToken = default) {
        var buffer = new byte[_bufferSize];
        var totalBytes = _stream.Length;
        var bytesRead = 0L;

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                // 读取数据块
                var size = await _stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                if (size <= 0) break;

                // 写入数据
                await stream.WriteAsync(buffer.AsMemory(0, size), cancellationToken);

                // 更新已读取的字节数并报告进度
                bytesRead += size;
                var progressPercentage = (double)bytesRead / totalBytes;
                _progress.Report(Math.Min(1, progressPercentage));
            }
        }
        catch (Exception) when (_disposed)
        {
            // 如果已释放，忽略异常
            return;
        }
    }

    /// <summary>
    /// 重写释放方法，确保资源正确释放
    /// </summary>
    /// <param name="disposing">是否正在释放托管资源</param>
    protected override void Dispose(bool disposing) {
        _disposed = true;
        if (disposing)
        {
            _stream.Dispose();
        }

        base.Dispose(disposing);
    }
}