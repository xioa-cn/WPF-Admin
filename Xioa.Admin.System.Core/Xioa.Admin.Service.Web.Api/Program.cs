using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
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
using Microsoft.Extensions.Logging;

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

// 添加CORS服务
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader()
               .WithExposedHeaders("*");  // 允许暴露所有响应头
    });
});

// 添加授权策略
builder.Services.AddAuthorization(options =>
{
    // 添加刷新Token的策略
    options.AddPolicy(TokenConfig.OnlyRefreshToken, policy =>
        policy.RequireClaim(TokenType.Token_Type, TokenType.Refresh));
        
    // 添加访问Token的策略
    options.AddPolicy(TokenConfig.OnlyAccessToken, policy =>
        policy.RequireClaim(TokenType.Token_Type, TokenType.Access));
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
    
    // 添加开发环境下的Token验证设置
    if (builder.Environment.IsDevelopment())
    {
        options.RequireHttpsMetadata = false;  // 开发环境不要求HTTPS
    }
    
    options.TokenValidationParameters = new TokenValidationParameters
    {
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
    
    options.Events = new JwtBearerEvents
    {
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
        },
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Add("Token-Expired", "true");
            }
            return Task.CompletedTask;
        },
        // 添加错误处理
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            
            var result = JsonSerializer.Serialize(new
            {
                success = false,
                message = "未授权访问"
            });
            
            return context.Response.WriteAsync(result);
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

// // 配置 HTTP 请求管道
// builder.Services.Configure<FormOptions>(x =>
// {
//     x.ValueLengthLimit = int.MaxValue;
//     x.MultipartBodyLengthLimit = 100 * 1024 * 1024; // 100MB
//     x.MultipartHeadersLengthLimit = int.MaxValue;
//     x.BufferBodyLengthLimit = int.MaxValue;
// });
//
// // 配置 IIS 集成
// builder.Services.Configure<IISServerOptions>(options =>
// {
//     options.MaxRequestBodySize = 100 * 1024 * 1024; // 100MB
//     options.AllowSynchronousIO = true;
// });
//
// // 配置 HTTP 客户端
// builder.Services.AddHttpClient("DefaultClient", client =>
// {
//     client.Timeout = TimeSpan.FromMinutes(10);
// });

builder.AddAllServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    // 生产环境使用自定义错误处理
    app.UseExceptionHandler(builder =>
    {
        builder.Run(async context =>
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            
            var error = context.Features.Get<IExceptionHandlerFeature>();
            if (error != null)
            {
                var logger = context.RequestServices
                    .GetRequiredService<ILogger<Program>>();
                
                logger.LogError(error.Error, "发生未处理的异常");
                
                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    success = false,
                    message = app.Environment.IsDevelopment() 
                        ? error.Error.Message 
                        : "服务器内部错误",
                    details = app.Environment.IsDevelopment() 
                        ? error.Error.StackTrace 
                        : null,
                    path = context.Request.Path
                }));
            }
        });
    });
}

// 中间件顺序很重要
app.UseCors("AllowAll");  // CORS要在最前面

// 认证授权
app.UseAuthentication();
app.UseAuthorization();

// 自定义中间件
app.UseMiddleware<LoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();