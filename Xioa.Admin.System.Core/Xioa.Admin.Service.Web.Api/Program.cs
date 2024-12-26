using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Xioa.Admin.Service.Web.Api.Services;
using Microsoft.OpenApi.Models;
using NLog.Web;
using Xioa.Admin.Service.Web.Api.Controllers;
using Xioa.Admin.Service.Web.Api.Services.NlogService.Model;
using Xioa.Admin.Service.Web.Api.Services.NlogService.Service;
using Xioa.Admin.Service.Web.Api.Services.TokenService;
using Xioa.Admin.Service.Web.Api.Services.TokenService.Impl;
using Xioa.Admin.Service.Web.Api.Utils;

var builder = WebApplication.CreateBuilder(args);

// 初始化 NLog
builder.Logging.ClearProviders();
builder.Logging.AddNLog(LoggerConfig.LoggerFileName);
builder.Logging.SetMinimumLevel(LogLevel.Information);  // 设置最低日志级别为Information
// builder.Logging.AddFilter("Microsoft", LogLevel.Warning);    // Microsoft命名空间下的日志最低级别为Warning
// builder.Logging.AddFilter("System", LogLevel.Warning);       // System命名空间下的日志最低级别为Warning

// 添加控制器服务
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer(); //最小Api
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Xioa-Admin", Version = "v1" });

    // 定义安全方案
    var securityScheme = new OpenApiSecurityScheme {
        Name = "JWT Authentication",
        Description = "Enter JWT Bearer token **_only_**",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer", // 必须是小写
        BearerFormat = "JWT",
        Reference = new OpenApiReference {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };
    options.AddSecurityDefinition("Bearer", securityScheme);

    // 添加全局安全需求
    options.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            securityScheme, new[] { "Bearer" }
        }
    });
});

// 添加 JWT 身份验证服务
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        TokenBase.Audience = builder.Configuration["Jwt:Audience"];
        TokenBase.Issuer = builder.Configuration["Jwt:Issuer"];
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            RoleClaimType = TokenType.Token_Type,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
        options.Events = new JwtBearerEvents {
            OnTokenValidated = context =>
            {
                var tokenType = context.Principal?.FindFirst(TokenType.Token_Type)?.Value;
                if (tokenType != TokenType.Refresh) return Task.CompletedTask;
                // 获取请求的路由
                var requestedPath = context.HttpContext.Request.Path;

                // 检查请求的路由是否为刷新令牌的路由
                if (requestedPath != RefreshTokenApi.Router)
                {
                    context.Fail("Refresh token used in the wrong context.");
                }

                return Task.CompletedTask;
            }
        };
    });
//Token 策略
builder.Services.ConfigureTokenServices();

builder.AddAllServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ExceptionHandlingMiddleware>(); // 注册异常处理中间件
app.UseMiddleware<LoggingMiddleware>(); // 注册接口日志中间件

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();