using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.IO;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Forms;
using System.Threading.Tasks;
using Xioa.Admin.Core.Views.ScreenRecording.Utils;
using Xioa.Admin.Core.Views.ScreenRecording.Window;

namespace Xioa.Admin.Core.Views.ScreenRecording.ViewModel
{
    public partial class ScreenRecordingViewModel : Xioa.Admin.Core.Services.ViewModels.ViewModelBase
    {
        public ICommand SetRegionCommand { get; private set; }
        public ICommand SelectSavePathCommand { get; private set; }
        public bool IsRecording { get; private set; }
        [ObservableProperty] public string _RecordingTime;
        public Rect RecordingRegion { get; set; }
        public string SavePath { get; set; }

        private DispatcherTimer timer;
        private Stopwatch stopwatch;
        private Process ffmpegProcess;

        public ScreenRecordingViewModel()
        {
            SetRegionCommand = new RelayCommand(SetRegion);
            SelectSavePathCommand = new RelayCommand(SelectSavePath);
            timer = new DispatcherTimer();
            stopwatch = new Stopwatch();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            //stopwatch.Start();
            RecordingTime = "00:00:00";
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            RecordingTime = stopwatch.Elapsed.ToString(@"hh\:mm\:ss");
        }
        [ObservableProperty]
        private bool _startStatus = true;
        public bool Start() => StartStatus;
        [RelayCommand(CanExecute = nameof(Start))]
        private void StartRecording()
        {
            StartStatus = false;
            if (string.IsNullOrEmpty(SavePath))
            {
                SelectSavePath();


                if (string.IsNullOrEmpty(SavePath))
                {
                    System.Windows.MessageBox.Show("存储路径为空！");
                    StartStatus = true;
                    return;
                }
                
                
            }
            timer.Start();
            stopwatch.Reset();
            stopwatch.Start();
            Task.Run(() => { ScreenRecordHelper.Start(SavePath); });
        }
        [RelayCommand]
        private void StopRecording()
        {
            StartStatus = true;
            ScreenRecordHelper.Stop();
            stopwatch.Stop();
            timer.Stop();
            RecordingTime = "00:00:00";
        }
        public ScreenRecordingTimer screenRecordingTimer;

        public static bool ElfOpen = true;

        public bool ElfStatus() => ElfOpen;

        [RelayCommand(CanExecute = nameof(ElfStatus))]
        public void OpenElf()
        {
            ElfOpen = false;
            screenRecordingTimer = new ScreenRecordingTimer()
            {
                DataContext = this
            };

            screenRecordingTimer.Show();
        }

        private void SetRegion()
        {
            
        }

        private void SelectSavePath()
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Filter = "MP4 file (*.mp4)|*.mp4";
            if (saveFileDialog.ShowDialog() == true)
            {
                SavePath = saveFileDialog.FileName;
            }
        }
    }


}