using System;
using System.Drawing;
using System.IO;

namespace DataAcquisition.Core.Views.QrCode.Utils;

/// <summary>
/// @author Xioa
/// @date  2024年12月3日
/// </summary>
public class QrCodeHelper
{
    private static byte[] BitmapToByte(Bitmap bitmap)
    {
        System.IO.MemoryStream ms = new System.IO.MemoryStream();
        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
        ms.Seek(0, System.IO.SeekOrigin.Begin);
        byte[] bytes = new byte[ms.Length];
        ms.Read(bytes, 0, bytes.Length);
        ms.Dispose();
        return bytes;
    }

    public static object? CreateQRCode(string msg, int version, string? iconFile = null)
    {
        QRCoder.QRCodeGenerator qRCodeGenerator = new QRCoder.QRCodeGenerator();
        QRCoder.QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode(msg, QRCoder.QRCodeGenerator.ECCLevel.M, true,
            true, QRCoder.QRCodeGenerator.EciMode.Utf8, version);
        QRCoder.SvgQRCode svgQrCode = new QRCoder.SvgQRCode(qRCodeData);
        if (iconFile is not null)
        {
            Bitmap iconBitmap = new Bitmap(iconFile);
            var iconByte = BitmapToByte(iconBitmap);
            QRCoder.SvgQRCode.SvgLogo icon = new QRCoder.SvgQRCode.SvgLogo(iconByte, 15);

            var svgString = svgQrCode.GetGraphic(new Size(200, 200), false,
                QRCoder.SvgQRCode.SizingMode.WidthHeightAttribute, icon);

            string documentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SVG", $"{DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒")}.svg");
            File.WriteAllText(documentPath, svgString);
            var fs = File.OpenRead(documentPath);
            return documentPath;
        }
        else
        {
            var svgString = svgQrCode.GetGraphic(new Size(300, 300), false);

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


        //  var bytes=System.Text.Encoding.Default.GetBytes(svgString);  
        // // var inputBytes = System.Convert.FromBase64String(svgString);
        // using MemoryStream stream = new MemoryStream(fs);

        //return null;
        // var ret = QRCoder.SvgQRCodeHelper.GetQRCode(msg, 10, 
        //     "#333333", "#FFFFFF", 
        //     QRCoder.QRCodeGenerator.ECCLevel.M, true,
        //     true, QRCoder.QRCodeGenerator.EciMode.Utf8, -1, true, QRCoder.SvgQRCode.SizingMode.WidthHeightAttribute, icon);
        // using MemoryStream stream = new MemoryStream(bitmap);
        // return new Bitmap(stream);
    }
}