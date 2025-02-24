using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Poll.Core.Configuration.Models;
using Poll.Core.Interfaces;
using Poll.Core.Consts.Authorization;
using Poll.Core.Entities.Ldap;
using Poll.Core.Exceptions;

namespace Poll.Core.Services.Authorization;

public class AuthorizationService : IAuthorizationService
{
    private readonly ILdapService _ldapService;
    private static TokenValidationParameters? _validationParameters;
    private readonly JwtSettings _jwtSettings;

    public AuthorizationService(ILdapService ldapService, IOptions<JwtSettings> options)
    {
        _ldapService = ldapService;
        _jwtSettings = options.Value;
        _validationParameters ??= new TokenValidationParameters
        {
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = true,
            ValidateIssuer = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = _jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(_jwtSettings.Key)),
            ValidateIssuerSigningKey = true,
        };
    }

    public async Task<LoginResult> Login(string login, string password, CancellationToken ct)
    {
        var user = await _ldapService.Login(login, password);
        if (user is null)
        {
            throw new AppException("Неправильный логин или пароль");
        }

        var accessToken = CreateJwtToken(user.ObjectGuid, login, user.Role, true);
        var refreshToken = CreateJwtToken(user.ObjectGuid, login, user.Role, false);
        return new LoginResult(accessToken, refreshToken);
    }

    public LoginResult RefreshToken(string refreshToken)
    {
        if (!ValidateToken(refreshToken, out string? id, out string? login, out UserRole role))
        {
            throw new AppException("Неверный токен обновления");
        }

        var accessToken = CreateJwtToken(id, login, role, true);
        var newRefreshToken = CreateJwtToken(id, login, role, false);
        return new LoginResult(accessToken, newRefreshToken);
    }

    private string CreateJwtToken(string id, string login, UserRole role, bool isAccessToken)
    {
        var claims = new List<Claim>
        {
            new(Claims.RoleClaimType, role.ToString()),
            new(Claims.LoginClaimType, login),
            new(Claims.IdentifierClaimType, id)
        };

        var claimsIdentity = new ClaimsIdentity(claims, "Token", Claims.LoginClaimType, Claims.RoleClaimType);
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = new SecurityTokenDescriptor
        {
            Audience = _jwtSettings.Audience,
            Issuer = _jwtSettings.Issuer,
            Subject = claimsIdentity,
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(Convert.FromBase64String(_jwtSettings.Key)),
                    SecurityAlgorithms.HmacSha256Signature),
            NotBefore = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(isAccessToken
                ? _jwtSettings.MinutesLifeTime
                : _jwtSettings.MinutesRefreshTokenLifeTime)
        };

        var securityToken = tokenHandler.CreateToken(token);
        var jwt = tokenHandler.WriteToken(securityToken);
        return jwt;
    }

    public bool ValidateToken(string token, [NotNullWhen(true)] out string? id, [NotNullWhen(true)] out string? login, 
         out UserRole role)
    {
        login = null;
        role = UserRole.None;
        id = null;

        try
        {
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, _validationParameters, out SecurityToken validatedToken);
            var securityToken = validatedToken as JwtSecurityToken;
            var idClaimValue = securityToken?.Claims.First(x => x.Type == Claims.IdentifierClaimType).Value;
            var loginClaimValue = securityToken?.Claims.First(x => x.Type == Claims.LoginClaimType).Value;
            var roleClaimValue = securityToken?.Claims.First(x => x.Type == Claims.RoleClaimType).Value;
            if (string.IsNullOrEmpty(loginClaimValue) || string.IsNullOrEmpty(idClaimValue) 
                                                      || string.IsNullOrEmpty(roleClaimValue))
            {
                return false;
            }

            id = idClaimValue;
            login = loginClaimValue;
            return Enum.TryParse(roleClaimValue, out role);
        }
        catch (Exception)
        {
            return false;
        }
    }
}