using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xioa.Admin.Core.Views.NAudioPlayer.ViewModel;
using Microsoft.Win32;

namespace Xioa.Admin.Core.Views.NAudioPlayer;

public partial class NAudioPlayerPage : Page {
    private NAudioPlayerViewModel viewModel;

    public NAudioPlayerPage() {
        InitializeComponent();
        this.Loaded += (sender, args) =>
        {
            if (this.DataContext is NAudioPlayerViewModel value)
            {
                viewModel = value;
            }
        };
    }

    private void PlayButton_Click(object sender, RoutedEventArgs e) {
        viewModel.Play();
    }

    private void StopButton_Click(object sender, RoutedEventArgs e) {
        viewModel.Stop();
    }

    private void LoadButton_Click(object sender, RoutedEventArgs e) {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "音频文件 (*.mp3;*.wav)|*.mp3;*.wav";
        if (openFileDialog.ShowDialog() == true)
        {
            viewModel.Load(openFileDialog.FileName);
        }
    }
}