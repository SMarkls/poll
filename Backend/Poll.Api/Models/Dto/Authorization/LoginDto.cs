namespace Poll.Api.Models.Dto.Authorization;

public class LoginDto
{
    public required string Login { get; init; }
    public required string Password { get; init; }
}