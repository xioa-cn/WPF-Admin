namespace Xioa.Admin.Service.Web.Api.Services.TokenService.Models;

public class RefreshTokenRequest
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}