using System.Diagnostics.CodeAnalysis;
using Poll.Core.Entities.Ldap;

namespace Poll.Core.Services.Authorization;

public interface IAuthorizationService
{
    Task<LoginResult> Login(string login, string password, CancellationToken ct);
    LoginResult RefreshToken(string refreshToken);

    bool ValidateToken(string token, [NotNullWhen(true)] out string? id, [NotNullWhen(true)] out string? login,
        [NotNullWhen(true)] out UserRole role);
}