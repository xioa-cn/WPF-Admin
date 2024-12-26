using Xioa.Admin.Service.Web.Api.Services.TokenService.Models;

namespace Xioa.Admin.Service.Web.Api.Services.TokenService.Impl;

public interface IJwtAuthManager
{
    TokenResponse? Authenticate(string username, string password);
    string? Refresh(string refreshToken, string accessToken);
}