using System;
using System.Threading.Tasks;
using System.Windows;
using ZXing.Aztec.Internal;

namespace Xioa.Admin.Core.Services.Tokens.Http;

public static class LoginRequestService
{
    public class LoginDao
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }

    public static async Task Login(string userName = "user", string password = "password")
    {
        try
        {
            var response = await ApplicationAxios.Axios.PostAsync<LoginDao>("/Authentication/token", new
            {
                UserName = userName,
                Password = password
            });

            if (response != null)
            {
                await Task.Run(() => RefreshTokenMethod(response));
            }
        }
        catch (Exception ex)
        {
            // 处理登录错误
            MessageBox.Show($"登录失败: {ex.Message}");
        }
    }

    public static void RefreshTokenMethod(LoginDao token)
    {
        if (token == null) return;


        Tokens.Instance.AccessToken = token.AccessToken;
        Tokens.Instance.RefreshToken = token.RefreshToken;

    }
}