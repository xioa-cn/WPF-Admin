 using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataAcquisition.Core.Views.PrintView.Models;

namespace DataAcquisition.Core.Views.PrintView.Services;

public class ZebraPrintService : IPrintService
{
    public async Task<bool> PrintZpl(string printerName, string zplContent)
    {
        try
        {
            using (var client = new RawPrinterHelper())
            {
                byte[] data = Encoding.UTF8.GetBytes(zplContent);
                return await client.SendBytesToPrinter(printerName, data);
            }
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> PrintLabel(string printerName, ZplTemplate template, Dictionary<string, string> data)
    {
        if (template == null) return false;

        try
        {
            string zplContent = template.GenerateZpl(data);
            return await PrintZpl(printerName, zplContent);
        }
        catch (Exception)
        {
            return false;
        }
    }

    public List<string> GetZebraPrinters()
    {
        var printers = new List<string>();
        foreach (string printer in PrinterSettings.InstalledPrinters)
        {
            try
            {
                var settings = new PrinterSettings { PrinterName = printer };
                if (settings.IsValid)
                {
                    printers.Add(printer);
                }
            }
            catch
            {
                // Skip invalid printers
            }
        }
        return printers;
    }

    public async Task<bool> TestConnection(string printerName)
    {
        try
        {
            using (var client = new RawPrinterHelper())
            {
                // 使用CancellationTokenSource来实现超时
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                
                // Send a simple test command to the printer
                string testCommand = "~HI\r\n";
                byte[] data = Encoding.UTF8.GetBytes(testCommand);
                
                return await Task.Run(async () =>
                {
                    try
                    {
                        return await client.SendBytesToPrinter(printerName, data);
                    }
                    catch (OperationCanceledException)
                    {
                        return false;
                    }
                }, cts.Token);
            }
        }
        catch (Exception)
        {
            return false;
        }
    }

    private class RawPrinterHelper : IDisposable
    {
        [DllImport("winspool.drv", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern bool OpenPrinter(string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.drv", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, int dwCount, out int dwWritten);

        private IntPtr _printerHandle;

        public bool TestPrinter(string printerName)
        {
            return OpenPrinter(printerName.Normalize(), out var handle, IntPtr.Zero) 
                   && ClosePrinter(handle);
        }

        public async Task<bool> SendBytesToPrinter(string printerName, byte[] data)
        {
            if (!OpenPrinter(printerName.Normalize(), out _printerHandle, IntPtr.Zero))
            {
                return false;
            }

            return await Task.Run(() =>
            {
                try
                {
                    var pUnmanagedBytes = Marshal.AllocCoTaskMem(data.Length);
                    Marshal.Copy(data, 0, pUnmanagedBytes, data.Length);
                    int dwWritten;
                    var success = WritePrinter(_printerHandle, pUnmanagedBytes, data.Length, out dwWritten);
                    Marshal.FreeCoTaskMem(pUnmanagedBytes);
                    return success;
                }
                finally
                {
                    ClosePrinter(_printerHandle);
                }
            });
        }

        public void Dispose()
        {
            if (_printerHandle != IntPtr.Zero)
            {
                ClosePrinter(_printerHandle);
                _printerHandle = IntPtr.Zero;
            }
        }
    }

    public List<string> GetPrinters()
    {
        throw new NotImplementedException();
    }

    public void Print(string printerName, string content)
    {
        throw new NotImplementedException();
    }

    public void PreviewContent(string content)
    {
        throw new NotImplementedException();
    }
}