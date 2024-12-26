using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Xioa.Admin.Service.Web.Api.Services.TokenService;
using Xioa.Admin.Service.Web.Api.Services.TokenService.Impl;
using Xioa.Admin.Service.Web.Api.Services.TokenService.Models;

namespace Xioa.Admin.Service.Web.Api.Controllers {
    public static class RefreshTokenApi {
        public const string Router = "/api/Authentication/refresh-token";
    }
    
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase {
        private readonly IJwtAuthManager _jwtAuthManager;

        public AuthenticationController(IJwtAuthManager jwtAuthManager) {
            _jwtAuthManager = jwtAuthManager;
        }

        [HttpPost("token")]
        public IActionResult GenerateToken(UserCredentials credentials) {
            var tokens = _jwtAuthManager.Authenticate(credentials.UserName, credentials.Password);
            if (tokens == null) return Unauthorized();
            return Ok(tokens);
        }

        [HttpPost("refresh-token")]
        [Authorize(Policy = TokenConfig.OnlyRefreshToken)]
        public IActionResult RefreshToken(RefreshTokenRequest request) {
            var accessToken = _jwtAuthManager.Refresh(request.RefreshToken, request.AccessToken);
            if (accessToken == null) return Unauthorized();
            return Ok(new { AccessToken = accessToken });
        }

        [HttpGet("test")]
        [Authorize(Policy = TokenConfig.OnlyAccessToken)]
        public IActionResult TestEndpoint() {
            return Ok("Access granted. You are authorized.");
        }
    }
}