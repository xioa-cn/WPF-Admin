using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Xioa.Admin.Core.Views.PrintView.Services;

public class RawPrinterHelper : IDisposable
{
    [DllImport("winspool.drv", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool OpenPrinter(string pPrinterName, out IntPtr phPrinter, IntPtr pDefault);

    [DllImport("winspool.drv", SetLastError = true)]
    private static extern bool ClosePrinter(IntPtr hPrinter);

    [DllImport("winspool.drv", SetLastError = true)]
    private static extern bool StartDocPrinter(IntPtr hPrinter, int level, ref DOCINFO di);

    [DllImport("winspool.drv", SetLastError = true)]
    private static extern bool EndDocPrinter(IntPtr hPrinter);

    [DllImport("winspool.drv", SetLastError = true)]
    private static extern bool StartPagePrinter(IntPtr hPrinter);

    [DllImport("winspool.drv", SetLastError = true)]
    private static extern bool EndPagePrinter(IntPtr hPrinter);

    [DllImport("winspool.drv", SetLastError = true)]
    private static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, int dwCount, out int dwWritten);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct DOCINFO
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pDocName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pOutputFile;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pDataType;
    }

    private IntPtr _printerHandle;
    private bool _disposed;

    public async Task<bool> SendBytesToPrinter(string printerName, byte[] bytes)
    {
        if (bytes == null || bytes.Length == 0) return false;

        try
        {
            if (!OpenPrinter(printerName.Normalize(), out _printerHandle, IntPtr.Zero))
            {
                return false;
            }

            var di = new DOCINFO
            {
                pDocName = "Zebra Label Document",
                pDataType = "RAW"
            };

            if (!StartDocPrinter(_printerHandle, 1, ref di))
            {
                ClosePrinter(_printerHandle);
                return false;
            }

            if (!StartPagePrinter(_printerHandle))
            {
                EndDocPrinter(_printerHandle);
                ClosePrinter(_printerHandle);
                return false;
            }

            return await Task.Run(() =>
            {
                var pUnmanagedBytes = Marshal.AllocCoTaskMem(bytes.Length);
                Marshal.Copy(bytes, 0, pUnmanagedBytes, bytes.Length);

                try
                {
                    if (!WritePrinter(_printerHandle, pUnmanagedBytes, bytes.Length, out int bytesWritten))
                    {
                        return false;
                    }

                    return bytesWritten == bytes.Length;
                }
                finally
                {
                    Marshal.FreeCoTaskMem(pUnmanagedBytes);
                    EndPagePrinter(_printerHandle);
                    EndDocPrinter(_printerHandle);
                    ClosePrinter(_printerHandle);
                }
            });
        }
        catch (Exception)
        {
            if (_printerHandle != IntPtr.Zero)
            {
                ClosePrinter(_printerHandle);
            }
            return false;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            if (_printerHandle != IntPtr.Zero)
            {
                ClosePrinter(_printerHandle);
                _printerHandle = IntPtr.Zero;
            }
        }

        _disposed = true;
    }

    ~RawPrinterHelper()
    {
        Dispose(false);
    }
}
