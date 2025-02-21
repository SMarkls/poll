namespace Poll.Api.Models.Dto.Authorization;

/// <summary>
/// Объект передачи данных авторизации.
/// </summary>
public class LoginDto
{
    /// <summary>
    /// Логин.
    /// </summary>
    public required string Login { get; init; }

    /// <summary>
    /// Пароль.
    /// </summary>
    public required string Password { get; init; }
}