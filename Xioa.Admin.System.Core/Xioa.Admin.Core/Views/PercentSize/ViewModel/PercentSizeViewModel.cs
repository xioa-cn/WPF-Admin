using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Controls;

namespace Xioa.Admin.Core.Views.PercentSize.ViewModel;

public partial class PercentSizeViewModel : Xioa.Admin.Core.Services.ViewModels.ViewModelBase
{
    [ObservableProperty] private double _width = 50;
    [ObservableProperty] private double _height = 50;

    [ObservableProperty] private double _Spacing = 0;

    [ObservableProperty] private Orientation _Orientation = Orientation.Horizontal;
    private bool _OrientationHorizeontal = true;
    public bool OrientationHorizeontal
    {
        get { return _OrientationHorizeontal; }
        set
        {
            if (value)
            {
                Orientation = Orientation.Horizontal;
            }
            else
            {
                Orientation = Orientation.Vertical;
            }

            SetProperty(ref _OrientationHorizeontal, value);
        }
    }
}