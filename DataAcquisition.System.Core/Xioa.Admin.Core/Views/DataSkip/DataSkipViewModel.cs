using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Xioa.Admin.Core.Views.DataSkip.Models;

namespace Xioa.Admin.Core.Views.DataSkip;

public partial class DataSkipViewModel : ObservableObject
{
    /// <summary>
    /// 存储所有数据项的集合
    /// </summary>
    private List<DataItem>? _allItems;

    /// <summary>
    /// 搜索文本，用于过滤数据
    /// </summary>
    [ObservableProperty] private string _searchText = string.Empty;

    /// <summary>
    /// 当前页显示的数据集合
    /// </summary>
    [ObservableProperty] private ObservableCollection<DataItem>? _currentPageData;

    /// <summary>
    /// 当前页码，从1开始
    /// </summary>
    [ObservableProperty] private int _currentPage = 1;

    /// <summary>
    /// 总页数
    /// </summary>
    [ObservableProperty] private int _totalPages;

    /// <summary>
    /// 总记录数
    /// </summary>
    [ObservableProperty] private int _totalItems;

    /// <summary>
    /// 当前选择的每页显示记录数
    /// </summary>
    [ObservableProperty] private int _selectedPageSize;

    /// <summary>
    /// 可选的每页显示记录数列表
    /// </summary>
    public List<int> PageSizes { get; } = new List<int> { 10, 20, 50, 100 };

    /// <summary>
    /// 构造函数，初始化数据和默认设置
    /// </summary>
    public DataSkipViewModel()
    {
        // 初始化数据
        _allItems = GenerateSampleData();
        SelectedPageSize = PageSizes[0]; // 默认每页10条
        UpdatePagingInfo();
        LoadCurrentPageData();
    }

    /// <summary>
    /// 当每页显示记录数改变时触发
    /// </summary>
    /// <param name="value">新的每页显示记录数</param>
    partial void OnSelectedPageSizeChanged(int value)
    {
        CurrentPage = 1; // 重置到第一页
        UpdatePagingInfo();
        LoadCurrentPageData();
    }

    /// <summary>
    /// 当搜索文本改变时触发
    /// </summary>
    /// <param name="value">新的搜索文本</param>
    partial void OnSearchTextChanged(string value)
    {
        CurrentPage = 1; // 重置到第一页
        UpdatePagingInfo();
        LoadCurrentPageData();
    }

    /// <summary>
    /// 执行搜索命令
    /// </summary>
    [RelayCommand]
    private void Search()
    {
        CurrentPage = 1;
        UpdatePagingInfo();
        LoadCurrentPageData();
    }

    /// <summary>
    /// 跳转到第一页命令
    /// </summary>
    [RelayCommand]
    private void FirstPage()
    {
        if (CurrentPage != 1)
        {
            CurrentPage = 1;
            LoadCurrentPageData();
        }
    }

    /// <summary>
    /// 跳转到上一页命令
    /// </summary>
    [RelayCommand]
    private void PreviousPage()
    {
        if (CurrentPage > 1)
        {
            CurrentPage--;
            LoadCurrentPageData();
        }
    }

    /// <summary>
    /// 跳转到下一页命令
    /// </summary>
    [RelayCommand]
    private void NextPage()
    {
        if (CurrentPage >= TotalPages) return;
        CurrentPage++;
        LoadCurrentPageData();
    }

    /// <summary>
    /// 跳转到最后一页命令
    /// </summary>
    [RelayCommand]
    private void LastPage()
    {
        if (CurrentPage == TotalPages) return;
        CurrentPage = TotalPages;
        LoadCurrentPageData();
    }

    /// <summary>
    /// 更新分页信息，包括总记录数和总页数
    /// </summary>
    private void UpdatePagingInfo()
    {
        var filteredItems = GetFilteredItems();
        if (filteredItems != null) TotalItems = filteredItems.Count;
        TotalPages = (int)Math.Ceiling(TotalItems / (double)SelectedPageSize);

        // 确保当前页不超过总页数
        if (CurrentPage > TotalPages)
        {
            CurrentPage = Math.Max(1, TotalPages);
        }
    }

    /// <summary>
    /// 加载当前页的数据
    /// </summary>
    private void LoadCurrentPageData()
    {
        var filteredItems = GetFilteredItems();
        if (filteredItems == null) return;
        var pageData = filteredItems
            .Skip((CurrentPage - 1) * SelectedPageSize)
            .Take(SelectedPageSize)
            .ToList();

        CurrentPageData = new ObservableCollection<DataItem>(pageData);
    }

    /// <summary>
    /// 根据搜索条件获取过滤后的数据
    /// </summary>
    /// <returns>过滤后的数据集合</returns>
    private List<DataItem>? GetFilteredItems()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
            return _allItems;

        // 查询符合条件的数据 切忽略大小写
        return _allItems?
            .Where(x =>
                x is { Status: not null, Description: not null, Name: not null } &&
                (x.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 x.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 x.Status.Contains(SearchText, StringComparison.OrdinalIgnoreCase)))
            .ToList();
    }

    /// <summary>
    /// 生成示例数据
    /// </summary>
    /// <returns>包含示例数据的集合</returns>
    private static List<DataItem> GenerateSampleData()
    {
        var items = new List<DataItem>();
        var random = new Random();
        var statuses = new[] { "活跃", "已完成", "待处理", "已取消" };

        for (var i = 1; i <= 150; i++)
        {
            items.Add(new DataItem
            {
                Id = i,
                Name = $"项目 {i}",
                Description = $"这是项目 {i} 的详细描述信息",
                CreateTime = DateTime.Now.AddDays(-random.Next(1, 100)),
                Status = statuses[random.Next(statuses.Length)]
            });
        }

        return items;
    }
}