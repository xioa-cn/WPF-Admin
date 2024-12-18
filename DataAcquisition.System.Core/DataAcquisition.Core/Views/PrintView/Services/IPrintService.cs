 using System.Collections.Generic;

namespace DataAcquisition.Core.Views.PrintView.Services
{
    public interface IPrintService
    {
        /// <summary>
        /// 获取所有可用打印机
        /// </summary>
        List<string> GetPrinters();

        /// <summary>
        /// 打印内容
        /// </summary>
        /// <param name="printerName">打印机名称</param>
        /// <param name="content">要打印的内容</param>
        void Print(string printerName, string content);

        /// <summary>
        /// 预览打印内容
        /// </summary>
        /// <param name="content">要预览的内容</param>
        void PreviewContent(string content);
    }
}