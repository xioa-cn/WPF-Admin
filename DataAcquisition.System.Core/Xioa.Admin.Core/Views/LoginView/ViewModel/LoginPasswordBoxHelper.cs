using System;
using System.Windows;
using System.Windows.Controls;

namespace Xioa.Admin.Core.Views.LoginView.ViewModel;

public static class LoginPasswordBoxHelper
{
    public static string GetPassword(DependencyObject obj)
    {
        return (string)obj.GetValue(PasswordProperty);
    }

    public static void SetPassword(DependencyObject obj, string value)
    {
        obj.SetValue(PasswordProperty, value);
    }

    public static readonly DependencyProperty PasswordProperty =
        DependencyProperty.RegisterAttached("Password", typeof(string), typeof(LoginPasswordBoxHelper),
            new PropertyMetadata(""));

    public static bool GetIsPasswordBindingEnable(DependencyObject obj)
    {
        return (bool)obj.GetValue(IsPasswordBindingEnableProperty);
    }

    public static void SetIsPasswordBindingEnable(DependencyObject obj, bool value)
    {
        obj.SetValue(IsPasswordBindingEnableProperty, value);
    }

    public static readonly DependencyProperty IsPasswordBindingEnableProperty =
        DependencyProperty.RegisterAttached("IsPasswordBindingEnable", typeof(bool), typeof(LoginPasswordBoxHelper),
            new FrameworkPropertyMetadata(OnIsPasswordBindingEnabledChanged));

    private static void OnIsPasswordBindingEnabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
        var passwordBox = obj as PasswordBox;
        if (passwordBox != null)
        {
            passwordBox.PasswordChanged -= PasswordBoxPasswordChanged;
            if ((bool)e.NewValue)
            {
                passwordBox.PasswordChanged += PasswordBoxPasswordChanged;
            }
        }
    }

    static void PasswordBoxPasswordChanged(object sender, RoutedEventArgs e)
    {
        var passwordBox = (PasswordBox)sender;
        if (!String.Equals(GetPassword(passwordBox), passwordBox.Password))
        {
            SetPassword(passwordBox, passwordBox.Password);
        }
    }
}