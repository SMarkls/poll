namespace Poll.Api.Models.Dto.Authorization;

public class LoginResult
{
    public required string AccessToken { get; init; }
    public required uint ExpiresIn { get; init; }
}