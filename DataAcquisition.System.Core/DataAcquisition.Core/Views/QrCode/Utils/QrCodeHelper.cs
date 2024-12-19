using SkiaSharp;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using ZXing;
using ZXing.SkiaSharp;

namespace DataAcquisition.Core.Views.QrCode.Utils;

/// <summary>
/// @author Xioa
/// @date  2024年12月3日
/// </summary>
public static class QrCodeHelper
{
    /// <summary>
    /// 将 Image 对象转换为字节数组。
    /// </summary>
    /// <param name="img">要转换的 Image 对象。</param>
    /// <returns>表示图像的字节数组。</returns>
    private static byte[]? ImageToByte(Image img)
    {
        // 使用 ImageConverter 类将 Image 对象转换为字节数组。
        return new ImageConverter().ConvertTo(img, typeof(byte[])) as byte[];
    }

    /// <summary>
    /// 从字节数组图像表示中解析条形码。
    /// </summary>
    /// <param name="bytes">包含图像数据的字节数组。</param>
    /// <returns>解析出的条形码文本，如果解析失败则为 null。</returns>
    public static string? ParseBarcode(byte[]? bytes)
    {
        try
        {
            // 使用 SkiaSharp 解码图像字节数组。
            var bitmap = SKBitmap.Decode(bytes);
            // 尝试读取条形码。
            return ReaderBarcode(bitmap);
        }
        catch
        {
            // 忽略异常并返回 null。
        }

        return null;
    }

    /// <summary>
    /// 检查字符串是否非空且非 null。
    /// </summary>
    /// <param name="text">要检查的字符串。</param>
    /// <returns>如果字符串非空且非 null，则返回 true。</returns>
    public static bool IsNotEmpty(string? text)
    {
        // 使用 .NET 的 String.IsNullOrEmpty 方法来检查字符串。
        return !string.IsNullOrEmpty(text);
    }

    /// <summary>
    /// 检查字符串是否为 null、空或仅包含空白。
    /// </summary>
    /// <param name="text">要检查的字符串。</param>
    /// <returns>如果字符串为 null、空或仅包含空白，则返回 true。</returns>
    public static bool IsNullOrEmpty(string? text)
    {
        // 使用 .NET 的 String.IsNullOrWhiteSpace 方法来检查字符串。
        if (string.IsNullOrWhiteSpace(text))
        {
            return true;
        }

        // 进一步检查字符串是否为 "null" 文本。
        return text == "null";
    }

    /// <summary>
    /// 水平翻转位图图像。
    /// </summary>
    /// <param name="bmp">要翻转的 SKBitmap 对象。</param>
    /// <returns>翻转后的 SKBitmap 对象。</returns>
    private static SKBitmap FlipBitmap(SKBitmap? bmp)
    {
        // 创建一个新的 SKBitmap 对象，用于存放翻转后的图像。
        var flipped = new SKBitmap(bmp.Width, bmp.Height, bmp.Info.ColorType, bmp.Info.AlphaType);

        // 使用 SKCanvas 在新的 SKBitmap 上绘图。
        using var canvas = new SKCanvas(flipped);
        // 设置转换矩阵，将图像向右移动，然后水平缩放 -1（即翻转）。
        canvas.Translate(bmp.Width, 0);
        canvas.Scale(-1, 1);
        // 将原始位图绘制到画布上。
        canvas.DrawBitmap(bmp, 0, 0);
        return flipped;
    }

    /// <summary>
    /// 从位图解码条形码，尝试正常和翻转的位图。
    /// </summary>
    /// <param name="bitmap">要从中解码条形码的 SKBitmap 对象。</param>
    /// <returns>解码出的条形码文本，如果解码失败则为 null。</returns>
    private static string? ReaderBarcode(SKBitmap? bitmap)
    {
        // 创建 ZXing 的条形码读取器。
        var reader = new BarcodeReader();
        // 首先尝试解码原始位图。
        var result = reader.Decode(bitmap);

        // 如果解码成功且结果非空，返回解码文本。
        if (result != null && IsNotEmpty(result.Text))
        {
            return result.Text;
        }

        // 如果原始位图解码失败，尝试解码翻转后的位图。
        var result2 = reader.Decode(FlipBitmap(bitmap));
        return result2?.Text;
    }

    /// <summary>
    /// 捕获指定窗口的屏幕内容并返回为字节数组。
    /// </summary>
    /// <param name="window">要捕获的窗口。</param>
    /// <returns>包含屏幕内容的字节数组，如果捕获失败则为 null。</returns>
    public static byte[]? CaptureScreen(Window window)
    {
        try
        {
            // 获取窗口的 DPI 设置。
            GetDpi(window, out var dpiX, out var dpiY);

            // 计算屏幕捕获的区域。
            var left = (int)(SystemParameters.WorkArea.Left);
            var top = (int)(SystemParameters.WorkArea.Top);
            var width = (int)(SystemParameters.WorkArea.Width / dpiX);
            var height = (int)(SystemParameters.WorkArea.Height / dpiY);

            // 创建一个 Bitmap 来存放屏幕截图。
            using var fullImage = new Bitmap(width, height);
            using var g = Graphics.FromImage(fullImage);
            // 从屏幕指定区域复制图像。
            g.CopyFromScreen(left, top, 0, 0, fullImage.Size, CopyPixelOperation.SourceCopy);
            // 将 Bitmap 转换为字节数组。
            return ImageToByte(fullImage);
        }
        catch
        {
            // 如果发生异常，返回 null。
            return null;
        }
    }

    /// <summary>
    /// 获取特定窗口的 DPI 设置。
    /// </summary>
    /// <param name="window">要获取 DPI 的窗口。</param>
    /// <param name="x">水平 DPI 的输出参数。</param>
    /// <param name="y">垂直 DPI 的输出参数。</param>
    private static void GetDpi(Window window, out float x, out float y)
    {
        // 获取窗口的句柄。
        var hWnd = new WindowInteropHelper(window).EnsureHandle();
        // 使用该句柄获取 Graphics 对象。
        var g = Graphics.FromHwnd(hWnd);

        // 计算 DPI。
        x = 96 / g.DpiX;
        y = 96 / g.DpiY;
    }

    /// <summary>
    /// 将 Bitmap 对象转换为字节数组。
    /// </summary>
    /// <param name="bitmap">要转换的 Bitmap 对象。</param>
    /// <returns>表示 Bitmap 的字节数组。</returns>
    private static byte[] BitmapToByte(Bitmap bitmap)
    {
        // 使用 MemoryStream 来保存 Bitmap 数据。
        System.IO.MemoryStream ms = new System.IO.MemoryStream();
        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
        ms.Seek(0, System.IO.SeekOrigin.Begin);
        byte[] bytes = new byte[ms.Length];
        ms.Read(bytes, 0, bytes.Length);
        ms.Dispose();
        return bytes;
    }

    /// <summary>
    /// 创建一个 SVG 文件的 QR 码，并可选择在中心包含一个图标。
    /// </summary>
    /// <param name="msg">要编码的消息。</param>
    /// <param name="version">QR 码的版本。</param>
    /// <param name="iconFile">中心图标的文件路径，如果不需要图标则为 null。</param>
    /// <returns>生成的 SVG 文件的路径。</returns>
    public static object? CreateQRCode(string msg, int version, string? iconFile = null)
    {
        // 创建 QR 码生成器。
        QRCoder.QRCodeGenerator qRCodeGenerator = new QRCoder.QRCodeGenerator();
        // 生成 QR 码数据。
        QRCoder.QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode(msg, QRCoder.QRCodeGenerator.ECCLevel.M, true,
            true, QRCoder.QRCodeGenerator.EciMode.Utf8, version);
        // 创建 SVG QR 码。
        QRCoder.SvgQRCode svgQrCode = new QRCoder.SvgQRCode(qRCodeData);
        if (iconFile is not null)
        {
            // 如果提供了图标文件，加载图标并将其转换为字节数组。
            Bitmap iconBitmap = new Bitmap(iconFile);
            var iconByte = BitmapToByte(iconBitmap);
            QRCoder.SvgQRCode.SvgLogo icon = new QRCoder.SvgQRCode.SvgLogo(iconByte, 15);

            // 生成带图标的 SVG QR 码。
            var svgString = svgQrCode.GetGraphic(new System.Drawing.Size(200, 200), false,
                QRCoder.SvgQRCode.SizingMode.WidthHeightAttribute, icon);

            // 保存 SVG 文件。
            string documentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SVG", $"{DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒")}.svg");
            File.WriteAllText(documentPath, svgString);
            var fs = File.OpenRead(documentPath);
            return documentPath;
        }
        else
        {
            // 生成不带图标的 SVG QR 码。
            var svgString = svgQrCode.GetGraphic(new System.Drawing.Size(300, 300), false);

            // 保存 SVG 文件。
            string documentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SVG");

            try
            {
                if (!Directory.Exists(documentPath))
                    Directory.CreateDirectory(documentPath);

                documentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SVG", $"{DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒")}.svg");
                //File.Create(documentPath);
                File.WriteAllText(documentPath, svgString);
            }
            catch (Exception ex)
            {

                //throw;
            }

            //var fs = File.OpenRead(documentPath);
            return documentPath;
        }
    }
}