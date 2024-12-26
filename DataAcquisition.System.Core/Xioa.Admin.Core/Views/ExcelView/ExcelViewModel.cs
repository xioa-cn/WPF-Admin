using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using CommunityToolkit.Mvvm.Input;
using Xioa.Admin.Core.Views.ExcelView.Model;
using OfficeOpenXml;
using System.IO;
using HandyControl.Controls;

namespace Xioa.Admin.Core.Views.ExcelView;

public partial class ExcelViewModel
{
    public ObservableCollection<ExcelTestModel> ExcelTestModels { get; set; }

    public ExcelViewModel()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        ExcelTestModels = CreateModel();
    }


    [RelayCommand]
    private void SaveExcel()
    {
        string file;
        FolderBrowserDialog dialog = new FolderBrowserDialog();
        dialog.Description = "请选择文件路径";
        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            file = dialog.SelectedPath;
        }
        else
        {
            return;
        }

        string endfile = Path.Combine(file, $"{DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒")}.xlsx");

        using var package = new ExcelPackage(new FileInfo(endfile));
        // 创建 sheet 可以创建多个 
        ExcelWorksheet sheet1 = package.Workbook.Worksheets.Add("ExcelModel");

        for (int i = 0; i < ExcelTestModels.Count; i++)
        {
            sheet1.Cells[i + 1, 1].Value = ExcelTestModels[i].Id;
            sheet1.Cells[i + 1, 2].Value = ExcelTestModels[i].Name;
            sheet1.Cells[i + 1, 3].Value = ExcelTestModels[i].Age;
            sheet1.Cells[i + 1, 4].Value = ExcelTestModels[i].Bid;
            sheet1.Cells[i + 1, 5].Value = ExcelTestModels[i].Adress;
            sheet1.Cells[i + 1, 6].Value = ExcelTestModels[i].StartTime;
            sheet1.Cells[i + 1, 7].Value = ExcelTestModels[i].EndTime;
        }
        
        package.Save();
        
        Growl.Success("导出成功");
    }


    private ObservableCollection<ExcelTestModel> CreateModel()
    {
        var result = new ObservableCollection<ExcelTestModel>();
        Random r = new Random();
        foreach (var item in Enumerable.Range(0, 10))
        {
            result.Add(new ExcelTestModel
            {
                Id = item,
                Name = $"姓名{item}",
                Age = r.Next(20, 50),
                Bid = $"ACC{DateTime.Now:O}",
                Adress = "New York No. 1 Lake ParkNew York No. 1 Lake Park",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now
            });
        }

        return result;
    }
}