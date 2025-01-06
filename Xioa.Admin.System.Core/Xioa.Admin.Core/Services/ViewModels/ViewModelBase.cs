using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Xioa.Admin.Core.Services.ViewModels;

public partial class ViewModelBase : ObservableObject, IDisposable {
    protected virtual void ReleaseUnmanagedResources() {
        
    }

    public void Dispose() {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }
}