using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Markup;
using MessageBox = System.Windows.MessageBox;
using TextBox = System.Windows.Controls.TextBox;

namespace Xioa.Admin.Core.Views.PrintView.Services
{
    public class StandardPrintService : IPrintService
    {
        private PrintDocument printDocument;
        private string contentToPrint;

        public StandardPrintService()
        {
            printDocument = new PrintDocument();
            printDocument.PrintPage += PrintDocument_PrintPage;
        }

        public List<string> GetZebraPrinters()
        {
            // 获取所有已安装的打印机
            return PrinterSettings.InstalledPrinters.Cast<string>().ToList();
        }

        public List<string> GetPrinters()
        {
            throw new NotImplementedException();
        }

        public void Print(string printerName, string content)
        {
            try
            {
                contentToPrint = content;
                printDocument.PrinterSettings.PrinterName = printerName;
                
                // 显示打印预览对话框
                PrintPreviewDialog previewDialog = new PrintPreviewDialog();
                previewDialog.Document = printDocument;
                
                if (previewDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    printDocument.Print();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打印出错: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            try
            {
                // 设置打印区域和字体
                Font printFont = new Font("Arial", 10);
                RectangleF printArea = e.MarginBounds;
                
                // 绘制内容
                e.Graphics.DrawString(contentToPrint, printFont, Brushes.Black, printArea);
                
                // 指示是否还有更多页面需要打印
                e.HasMorePages = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打印页面生成错误: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void PreviewContent(string content)
        {
            // 创建预览窗口
            Window previewWindow = new Window
            {
                Title = "打印预览",
                Width = 800,
                Height = 600,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            // 创建预览内容
            TextBox previewBox = new TextBox
            {
                Text = content,
                IsReadOnly = true,
                TextWrapping = TextWrapping.Wrap,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                Margin = new Thickness(10)
            };

            previewWindow.Content = previewBox;
            previewWindow.ShowDialog();
        }
    }
}
