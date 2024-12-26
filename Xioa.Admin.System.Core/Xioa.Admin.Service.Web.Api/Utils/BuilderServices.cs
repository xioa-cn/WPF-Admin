using Xioa.Admin.Service.Web.Api.Services.TokenService;
using Xioa.Admin.Service.Web.Api.Services.TokenService.Impl;

namespace Xioa.Admin.Service.Web.Api.Utils;

///<summary>
/// @author：XIOA (xioa.liu)
/// @date：2024-12-26
/// @belong-sln：Xioa.Admin.System.Core 
/// @desc：BuilderServices
///</summary>
public static class BuilderServices
{
    public static WebApplicationBuilder AddAllServices(this WebApplicationBuilder builder) {
        builder.Services.AddSingleton<IJwtAuthManager>(new JwtAuthManager(builder.Configuration["Jwt:Key"]));
        
        return builder;
    }
}