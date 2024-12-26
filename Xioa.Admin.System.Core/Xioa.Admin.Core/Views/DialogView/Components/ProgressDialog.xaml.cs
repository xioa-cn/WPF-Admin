using System;
using System.Windows;
using System.Windows.Controls;
using Xioa.Admin.Core.Views.DialogView.Model;
using Xioa.Admin.Core.Views.DialogView.ViewModel;
using HandyControl.Controls;
using HandyControl.Tools.Extension;

namespace Xioa.Admin.Core.Views.DialogView.Components;

public partial class ProgressDialog : UserControl
{
    public ProgressDialog()
    {
        this.DataContext = new ProgressDialogViewModel();
        InitializeComponent();
        //CloseAction = CloseAction_Method;
        //Dialog.Token = MessageToken.DialogPageToken;
    }

    private void CloseAction_Method()
    {
        //Result = "";
    }

  

    private void Close_Dialog(object sender, RoutedEventArgs e)
    {
        
    }
}