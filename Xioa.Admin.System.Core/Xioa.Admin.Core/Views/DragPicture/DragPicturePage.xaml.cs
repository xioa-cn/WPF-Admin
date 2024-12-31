using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Xioa.Admin.Core.Views.DragPicture;

public partial class DragPicturePage : Page
{
    public DragPicturePage()
    {
        InitializeComponent();
    }

    private void Image_DragOver(object sender, DragEventArgs e)
    {
        Debug.WriteLine("Drag over event triggered."); 
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effects = DragDropEffects.Copy;
        }
        else
        {
            e.Effects = DragDropEffects.None;
        }
        e.Handled = true;
    }

    private void Image_Drop(object sender, DragEventArgs e)
    {
        Debug.WriteLine("Drop event triggered."); 
        var files = (string[]?)e.Data.GetData(DataFormats.FileDrop);
        if (files == null || files.Length <= 0) return;
        string file = files[0];
        string extension = System.IO.Path.GetExtension(file).ToLower();
        var imageExtensions = new HashSet<string> { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
        if (imageExtensions.Contains(extension))
        {
            Debug.WriteLine($"Dropped file is an image: {file}"); 
            displayImage.Source = new BitmapImage(new Uri(file));
        }
        else
        {
            Debug.WriteLine("Dropped file is not an image."); 
        }
    }
}