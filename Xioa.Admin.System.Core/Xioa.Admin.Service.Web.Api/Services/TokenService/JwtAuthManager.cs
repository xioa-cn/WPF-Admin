using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Xioa.Admin.Service.Web.Api.Services.TokenService.Impl;
using Xioa.Admin.Service.Web.Api.Services.TokenService.Models;

namespace Xioa.Admin.Service.Web.Api.Services.TokenService;

public class JwtAuthManager : IJwtAuthManager {
        private readonly string _key;

        public JwtAuthManager(string key) {
            _key = key;
        }

        public TokenResponse? Authenticate(string username, string password) {
            var accessToken = GenerateToken(username, TokenTime.AccessTime, TokenType.Access); // 短期令牌
            var refreshToken = GenerateToken(username, TokenTime.RefreshTime, TokenType.Refresh); // 长期令牌

            return new TokenResponse {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        private string GenerateToken(string username, int minutes, string tokenType) {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(_key);
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(TokenType.Token_Type, tokenType) // 添加自定义声明
                }),
                Expires = DateTime.UtcNow.AddMinutes(minutes),
                Audience = TokenBase.Audience,
                Issuer = TokenBase.Issuer,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string Refresh(string refreshToken, string accessToken) {
            // 解析 refreshToken
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)),
                ValidateIssuer = true,
                ValidIssuer = TokenBase.Issuer,
                ValidateAudience = true,
                ValidAudience = TokenBase.Audience,
                ValidateLifetime = true, // 确保 refreshToken 未过期
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(refreshToken, validationParameters,
                    out SecurityToken validatedToken);
                var jwtToken = validatedToken as JwtSecurityToken;
                if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                        StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException("Invalid token");

                // 检查 token_type 是否为 'refresh'
                var tokenType = principal.FindFirst(TokenType.Token_Type)?.Value;
                if (tokenType != TokenType.Refresh)
                    throw new SecurityTokenException("Invalid token type");

                // 如果 refreshToken 有效，生成新的 accessToken
                return GenerateToken(principal.Identity.Name, TokenTime.AccessTime, TokenType.Access);
            }
            catch (Exception ex)
            {
                // 处理验证失败的情况
                Console.WriteLine($"Token validation failed: {ex.Message}");
                return null;
            }
        }

        private DateTime? GetTokenExpiration(string token) {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken != null) {
                var expClaim = jwtToken.Payload.Exp;
                if (expClaim.HasValue) {
                    // 将 exp 从 Unix 时间戳转换为 DateTime，并考虑本地时区
                    var expirationDateUtc = DateTimeOffset.FromUnixTimeSeconds(expClaim.Value).UtcDateTime;
                    var localTime = expirationDateUtc.ToLocalTime(); // 转换为本地时间
                    return localTime;
                }
            }
            return null;
        }
    }