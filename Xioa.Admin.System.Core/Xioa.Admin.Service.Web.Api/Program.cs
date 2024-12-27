using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
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
builder.Logging.SetMinimumLevel(LogLevel.Information);

// 添加控制器服务
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Xioa-Admin", Version = "v1" });

    // 定义安全方案
    var securityScheme = new OpenApiSecurityScheme {
        Name = "JWT Authentication",
        Description = "Enter JWT Bearer token **_only_**",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };
    options.AddSecurityDefinition("Bearer", securityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            securityScheme, new[] { "Bearer" }
        }
    });
});

// JWT配置
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
                var requestedPath = context.HttpContext.Request.Path;
                if (requestedPath != RefreshTokenApi.Router)
                {
                    context.Fail("Refresh token used in the wrong context.");
                }
                return Task.CompletedTask;
            }
        };
    });

// 配置 Kestrel
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    // 设置请求体大小限制为100MB
    serverOptions.Limits.MaxRequestBodySize = 100 * 1024 * 1024;
    serverOptions.Limits.MaxRequestBufferSize = 100 * 1024 * 1024;
    
    // 禁用请求速率限制
    serverOptions.Limits.MinRequestBodyDataRate = null;
    serverOptions.Limits.MinResponseDataRate = null;
    
    // 增加超时时间
    serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(10);
    serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(10);
});

// 配置 HTTP 请求管道
builder.Services.Configure<FormOptions>(x =>
{
    x.ValueLengthLimit = int.MaxValue;
    x.MultipartBodyLengthLimit = 100 * 1024 * 1024; // 100MB
    x.MultipartHeadersLengthLimit = int.MaxValue;
    x.BufferBodyLengthLimit = int.MaxValue;
});

// 配置 IIS 集成
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 100 * 1024 * 1024; // 100MB
    options.AllowSynchronousIO = true;
});

// 配置 HTTP 客户端
builder.Services.AddHttpClient("DefaultClient", client =>
{
    client.Timeout = TimeSpan.FromMinutes(10);
});

builder.AddAllServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 移除HTTPS重定向
// app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<LoggingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors();

app.MapControllers();

app.Run();