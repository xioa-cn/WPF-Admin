using System;

namespace Xioa.Admin.Core.Views.ExcelView.Model;

/// <summary>
/// @author Xioa
/// @date  2024年12月3日
/// </summary>
public class ExcelTestModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int Age { get; set; }
    public string? Bid { get; set; }
    public string? Adress { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}