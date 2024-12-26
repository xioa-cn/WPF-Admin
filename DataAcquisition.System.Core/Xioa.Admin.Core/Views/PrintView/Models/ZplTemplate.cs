using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Xioa.Admin.Core.Views.PrintView.Models;

public partial class ZplTemplate : ObservableObject
{
    [ObservableProperty]
    private string name;

    [ObservableProperty]
    private int width;      // 标签宽度（点）

    [ObservableProperty]
    private int height;     // 标签高度（点）

    [ObservableProperty]
    private string content;  // ZPL内容

    partial void OnContentChanged(string value)
    {
        UpdateFields();
    }

    private void UpdateFields()
    {
        if (string.IsNullOrEmpty(Content)) return;

        // 使用正则表达式找出所有的{FieldName}格式的占位符
        var matches = Regex.Matches(Content, @"\{([^}]+)\}");
        var existingFields = Fields.ToDictionary(f => f.Name, f => f);
        var newFields = new List<TemplateField>();

        foreach (Match match in matches)
        {
            var fieldName = match.Groups[1].Value;
            if (existingFields.ContainsKey(fieldName))
            {
                newFields.Add(existingFields[fieldName]);
                existingFields.Remove(fieldName);
            }
            else
            {
                newFields.Add(new TemplateField
                {
                    Name = fieldName,
                    DisplayName = fieldName,
                    Description = $"字段 {fieldName} 的值"
                });
            }
        }

        // 更新字段集合
        Fields.Clear();
        foreach (var field in newFields)
        {
            Fields.Add(field);
        }
    }

    [ObservableProperty]
    private string description; // 模板描述

    [ObservableProperty]
    private DateTime createTime; // 创建时间

    [ObservableProperty]
    private DateTime? lastModifiedTime; // 最后修改时间

    [ObservableProperty]
    private ObservableCollection<TemplateField> fields; // 模板字段

    public ZplTemplate()
    {
        Fields = new ObservableCollection<TemplateField>();
        CreateTime = DateTime.Now;
    }

    public string GenerateZpl(Dictionary<string, string> values)
    {
        string result = Content ?? string.Empty;
        if (values == null) return result;

        foreach (var field in Fields)
        {
            if (values.TryGetValue(field.Name, out string value))
            {
                result = result.Replace($"{{{field.Name}}}", value ?? string.Empty);
            }
        }
        return result;
    }

    public static ZplTemplate Create100x50()
    {
        var template = new ZplTemplate
        {
            Name = "100x50标签",
            Description = "基础的100x50mm标签模板，包含标题和条码",
            Width = 100,
            Height = 50,
            Content = @"^XA
^FO20,20^A0N,30,30^FD{Title}^FS
^FO20,60^BY2^BCN,40,Y,N,N^FD{Barcode}^FS
^XZ"
        };

        var fields = new[]
        {
            new TemplateField { Name = "Title", DisplayName = "标题", Description = "显示在标签顶部的文本" },
            new TemplateField { Name = "Barcode", DisplayName = "条码", Description = "显示为条码的文本", IsRequired = true }
        };
        foreach (var field in fields)
        {
            template.Fields.Add(field);
        }

        return template;
    }

    public static ZplTemplate CreateQRCode()
    {
        var template = new ZplTemplate
        {
            Name = "二维码标签",
            Description = "包含标题和二维码的标签模板",
            Width = 50,
            Height = 50,
            Content = @"^XA
^FO10,10^A0N,20,20^FD{Title}^FS
^FO10,40^BQN,2,4^FD{QRContent}^FS
^XZ"
        };

        var fields = new[]
        {
            new TemplateField { Name = "Title", DisplayName = "标题", Description = "显示在二维码上方的文本" },
            new TemplateField { Name = "QRContent", DisplayName = "二维码内容", Description = "编码到二维码中的内容", IsRequired = true }
        };
        foreach (var field in fields)
        {
            template.Fields.Add(field);
        }

        return template;
    }

    public static ZplTemplate CreateProductLabel()
    {
        var template = new ZplTemplate
        {
            Name = "产品标签",
            Description = "完整的产品标签模板，包含多个字段",
            Width = 100,
            Height = 60,
            Content = @"^XA
^FO10,10^A0N,25,25^FD{ProductName}^FS
^FO10,40^A0N,20,20^FD{Specification}^FS
^FO10,65^A0N,20,20^FD{Date}^FS
^FO10,90^BY2^BCN,40,Y,N,N^FD{SerialNumber}^FS
^XZ"
        };

        var fields = new[]
        {
            new TemplateField { Name = "ProductName", DisplayName = "产品名称", Description = "产品的名称", IsRequired = true },
            new TemplateField { Name = "Specification", DisplayName = "规格", Description = "产品规格" },
            new TemplateField { Name = "Date", DisplayName = "日期", Description = "生产或包装日期", DefaultValue = DateTime.Now.ToString("yyyy-MM-dd") },
            new TemplateField { Name = "SerialNumber", DisplayName = "序列号", Description = "产品序列号", IsRequired = true }
        };

        foreach (var field in fields)
        {
            template.Fields.Add(field);
        }

        return template;
    }
}

public partial class TemplateField : ObservableObject
{
    [ObservableProperty]
    private string name;

    [ObservableProperty]
    private string displayName;

    [ObservableProperty]
    private string defaultValue;

    [ObservableProperty]
    private string description;

    [ObservableProperty]
    private bool isRequired;

    [ObservableProperty]
    private string value;

    public TemplateField()
    {
        Value = DefaultValue ?? string.Empty;
    }

    partial void OnDefaultValueChanged(string value)
    {
        if (string.IsNullOrEmpty(Value))
        {
            Value = value ?? string.Empty;
        }
    }
}