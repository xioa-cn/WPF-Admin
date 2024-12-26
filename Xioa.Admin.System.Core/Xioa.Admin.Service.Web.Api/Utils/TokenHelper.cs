using Xioa.Admin.Service.Web.Api.Services;
using Xioa.Admin.Service.Web.Api.Services.TokenService;

namespace Xioa.Admin.Service.Web.Api.Utils;


public static class TokenHelper {
    public static IServiceCollection ConfigureTokenServices(this IServiceCollection services) {
        services.AddAuthorization(options =>
        {
            // 策略，允许访问短期令牌的资源
            options.AddPolicy(TokenConfig.OnlyAccessToken, policy =>
                policy.RequireClaim(TokenType.Token_Type, TokenType.Access));

            // 策略，允许访问长期令牌的资源
            options.AddPolicy(TokenConfig.OnlyRefreshToken, policy =>
                policy.RequireClaim(TokenType.Token_Type, TokenType.Refresh));
        });
        return services;
    }
}