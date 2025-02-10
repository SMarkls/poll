namespace Poll.Api.Models.Dto.Authorization;

/// <summary>
/// Результат авторизации.
/// </summary>
public class LoginResult
{
    /// <summary>
    /// Токен доступа.
    /// </summary>
    public required string AccessToken { get; init; }

    /// <summary>
    /// Timestamp с временем истечения действия токена.
    /// </summary>
    public required uint ExpiresIn { get; init; }
}