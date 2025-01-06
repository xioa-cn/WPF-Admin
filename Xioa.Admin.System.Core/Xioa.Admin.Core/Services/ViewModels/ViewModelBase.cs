using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Xioa.Admin.Core.Services.ViewModels;

public partial class ViewModelBase : ObservableObject, IDisposable {
    protected virtual void ReleaseUnmanagedResources() {
        // TODO 在此释放非托管资源
    }

    public void Dispose() {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }
}