using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Xioa.Admin.Core.Services.ViewModels;

public partial class ViewModelBase : ObservableObject, IDisposable
{
    // // 典型的生命周期：
    // 1. 创建对象 -> InitializeAsync (一次性初始化)
    // 2. 页面显示 -> OnAppearing (可能多次)
    // 3. 页面隐藏 -> OnDisappearing
    // 4. 再次显示 -> OnAppearing
    // // ... 循环 2-4 步骤
    protected virtual Task OnDisappearing()
    {
        return Task.CompletedTask;
    }

    public virtual Task OnAppearing()
    {
        return Task.CompletedTask;
    }

    protected virtual Task InitializedAsync()
    {
        return Task.CompletedTask;
    }

    #region 资源释放

    private bool _isDisposed;

    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed) return;
        if (disposing)
        {
            // 释放托管资源
        }

        // 释放非托管资源
        _isDisposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}