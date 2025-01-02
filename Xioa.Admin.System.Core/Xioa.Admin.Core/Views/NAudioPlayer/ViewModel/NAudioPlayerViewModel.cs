using System;
using NAudio.Wave;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;

namespace Xioa.Admin.Core.Views.NAudioPlayer.ViewModel;

public partial class NAudioPlayerViewModel : ObservableObject {
    private IWavePlayer? wavePlayer;
    private WaveStream? audioStream;

    private DispatcherTimer timer;
    private bool _isLooping;

    private HttpClient httpClient = new HttpClient();

    public NAudioPlayerViewModel() {
        wavePlayer = new WaveOutEvent() {
            DesiredLatency = 1000
        };
        timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromMilliseconds(500);
        timer.Tick += Timer_Tick;
        wavePlayer.PlaybackStopped += OnPlaybackStopped;
    }

    private void OnPlaybackStopped(object? sender, StoppedEventArgs e) {
        if (_isLooping && audioStream != null)
        {
            audioStream.Position = 0;
            wavePlayer.Play();
        }
    }

    public bool IsLooping {
        get => _isLooping;
        set => SetProperty(ref _isLooping, value);
    }

    public void Load(string fileName) {
        audioStream = new AudioFileReader(fileName);
        wavePlayer?.Init(audioStream);
        OnPropertyChanged(nameof(TotalTime));
        TotalTimeString = audioStream?.TotalTime.ToString(@"hh\:mm\:ss");
    }

    public void Play() {
        if (wavePlayer != null && audioStream != null)
        {
            wavePlayer.Play();
            timer.Start();
        }
    }

    public void Stop() {
        wavePlayer?.Stop();
        timer.Stop();
    }

    public void Dispose() {
        wavePlayer?.Dispose();
        audioStream?.Dispose();
    }

    private double _currentPosition;

    public double CurrentPosition {
        get { return _currentPosition; }
        set
        {
            if (_currentPosition != value)
            {
                if (audioStream is not null)
                    audioStream.CurrentTime = TimeSpan.FromSeconds(value);
                _currentPosition = value;
                OnPropertyChanged();
            }
        }
    }

    [ObservableProperty] private string? _currentTime;
    [ObservableProperty] private string? _totalTimeString;

    public double TotalTime => audioStream?.TotalTime.TotalSeconds ?? 0;

    private void Timer_Tick(object sender, EventArgs e) {
        if (audioStream != null)
        {
            CurrentPosition = audioStream.CurrentTime.TotalSeconds;
            CurrentTime = audioStream.CurrentTime.ToString(@"hh\:mm\:ss");
        }
    }

    [ObservableProperty] private string? _musicUrlPath;

    [RelayCommand]
    private async Task LoadMusic() {
        await
            LoadFromUrl(MusicUrlPath);
    }

    public async Task LoadFromUrl(string url) {
        try
        {
            var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            using (var networkStream = await response.Content.ReadAsStreamAsync())
            using (var memoryStream = new MemoryStream())
            {
                await networkStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0; // Reset position after copy

                if (audioStream != null)
                {
                    audioStream.Dispose();
                }

                audioStream = new StreamMediaFoundationReader(memoryStream);
                wavePlayer.Init(audioStream);
                OnPropertyChanged(nameof(TotalTime));
                TotalTimeString = audioStream.TotalTime.ToString(@"hh\:mm\:ss");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error loading audio from URL: " + ex.Message);
            // Consider notifying the user through the UI
        }
    }
}