using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xioa.Admin.Core.Services.Tokens;
using static Xioa.Admin.Core.Services.Tokens.Http.LoginRequestService;
using System.Diagnostics;
using System.Windows;
using GMap.NET.MapProviders;
using System.Net.Http;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Net;

namespace Xioa.Admin.Core.Views.RefreshTokens
{
    public partial class RefreshTokenViewModel : Xioa.Admin.Core.Services.ViewModels.ViewModelBase
    {
        [ObservableProperty]
        private bool _isRefreshing;

        private string? _accessToken;
        public string? AccessToken
        {
            get => _accessToken;
            set => SetProperty(ref _accessToken, value);
        }

        private string? _refreshToken;
        public string? RefreshToken
        {
            get => _refreshToken;
            set => SetProperty(ref _refreshToken, value);
        }

        public RefreshTokenViewModel()
        {
            // 初始化时从 Tokens 获取值
            AccessToken = Tokens.Instance.AccessToken;
            RefreshToken = Tokens.Instance.RefreshToken;

            // 监听 Tokens 的变化
            Tokens.Instance.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Tokens.AccessToken))
                {
                    AccessToken = Tokens.Instance.AccessToken;
                }
                else if (e.PropertyName == nameof(Tokens.RefreshToken))
                {
                    RefreshToken = Tokens.Instance.RefreshToken;
                }
            };
        }
        public static HttpStatusCode Status = HttpStatusCode.OK;
        [RelayCommand(CanExecute = nameof(CanRefreshToken))]
        private async Task RefreshTokenClick(string status)
        {
            try
            {
                IsRefreshing = true;
                Status = (HttpStatusCode)int.Parse(status);
                await Task.Delay(1000);

                try
                {
                    // 第一次尝试请求 当前方案写在拦截器里
                    var headers = new Dictionary<string, string>
                    {
                        { "Authorization", $"Bearer {Tokens.Instance.AccessToken}" }
                    };

                    var response = await ApplicationAxios.Axios.PostAsync<LoginDao>(
                        "/Authentication/refresh-token",
                        new
                        {
                            Tokens.Instance.AccessToken,
                            Tokens.Instance.RefreshToken,
                        },
                        headers);

                    if (response != null)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            RefreshTokenMethod(response);
                            MessageBox.Show("Token刷新成功！");
                        });
                    }
                }
                catch (HttpRequestException ex) when (ex.Message.Contains("403")&& Status == HttpStatusCode.OK)
                {
                    try
                    {
                        var refreshHeaders = new Dictionary<string, string>
                        {
                            { "Authorization", $"Bearer {Tokens.Instance.RefreshToken}" }
                        };

                        var refreshResponse = await ApplicationAxios.Axios.PostAsync<LoginDao>(
                            "/Authentication/refresh-token",
                            new
                            {
                                Tokens.Instance.AccessToken,
                                Tokens.Instance.RefreshToken,
                            },
                            refreshHeaders);

                        if (refreshResponse != null)
                        {
                            RefreshTokenMethod(refreshResponse);
                        }
                    }
                    catch (Exception retryEx)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show($"重试失败: {retryEx.Message}");
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"刷新Token失败: {ex.Message}");
                });
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        private bool CanRefreshToken() => !IsRefreshing;
    }
}
