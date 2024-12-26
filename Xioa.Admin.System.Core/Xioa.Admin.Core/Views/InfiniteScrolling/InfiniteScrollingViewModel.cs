using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Xioa.Admin.Core.Views.InfiniteScrolling;

/// <summary>
/// @author Xioa
/// @date  2024-12-26 09:08:47
/// </summary>
public class InfiniteScrollingViewModel : ObservableObject {
    public InfiniteScrollingViewModel() {
        Items = new ObservableCollection<string>();
        LoadInitialData();
    }

    private void LoadInitialData() {
        for (int i = 0; i < 20; i++)
        {
            Items.Add($"Item {i + 1}");
        }
    }

    private ObservableCollection<string> _items;
    private bool _canMove = true;
    private bool _isLoading;

    public ObservableCollection<string> Items {
        get => _items;
        set => SetProperty(ref _items, value);
    }

    public bool CanMove {
        get => _canMove;
        set => SetProperty(ref _canMove, value);
    }

    public bool IsLoading {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public async Task MoveData() {
        if (!CanMove || IsLoading) return;

        CanMove = false;
        IsLoading = true;

        try
        {
            await Task.Delay(1000);

            var startIndex = Items.Count;
            for (int i = 0; i < 10; i++)
            {
                Items.Add($"Item {startIndex + i + 1}");
            }

            CanMove = Items.Count < 100;
        }
        catch (Exception ex)
        {
            CanMove = true;
        }
        finally
        {
            IsLoading = false;
        }
    }
}