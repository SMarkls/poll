namespace Poll.Core.Services.Authorization;

public record LoginResult(string AccessToken, string RefreshToken);