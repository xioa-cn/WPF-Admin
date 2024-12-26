namespace Xioa.Admin.Service.Web.Api.Services.TokenService;

public static class TokenType {
    public static readonly string Access = "access";
    public static readonly string Refresh = "refresh";
    public static readonly string Token_Type = "token_type";
}

public static class TokenTime {
    public static readonly int AccessTime = 5;
    public static readonly int RefreshTime = 1440;
}

public static class TokenBase {
    public static string? Audience { get; set; }
    public static string? Issuer { get; set; }
}

public static class TokenConfig {
    public const string OnlyAccessToken = "OnlyAccessToken";
    public const string OnlyRefreshToken = "OnlyRefreshToken";
}

