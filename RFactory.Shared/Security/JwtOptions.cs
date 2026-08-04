namespace RFactory.Shared.Security;

/// <summary>
/// Strongly-typed JWT settings bound from the "Jwt" configuration section.
/// </summary>
public class JwtOptions
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpireMinutes { get; set; } = 60;
    public int RefreshTokenExpireDays { get; set; } = 7;
}
