using System;

namespace DataAcquisition.Core.Views.DataSkip.Models;


public class DataItem
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreateTime { get; set; }
    public string? Status { get; set; }
} 