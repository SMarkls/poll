namespace Poll.Core.Configuration.Models;

public class JwtSettings
{
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required string Key { get; init; }
    public required uint MinutesLifeTime { get; init; }
    public required uint MinutesRefreshTokenLifeTime { get; init; }
}