using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Poll.Api.Filters;
using Poll.Api.Models.Dto.Authorization;
using Poll.Api.Swagger.Attributes;
using Poll.Core.Configuration.Models;
using IAuthorizationService = Poll.Core.Services.Authorization.IAuthorizationService;

namespace Poll.Api.Controllers;

/// <summary>
/// Контроллер авторизации.
/// </summary>
[Route("[controller]/[action]")]
public class AuthorizationController : BaseController
{
    private readonly IAuthorizationService _authorizationService;
    private readonly JwtSettings _jwtSettings;

    public AuthorizationController(IAuthorizationService authorizationService, IOptions<JwtSettings> options)
    {
        _jwtSettings = options.Value;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Авторизоваться.
    /// </summary>
    /// <param name="dto">Логин и пароль.</param>
    /// <returns>Токен доступа и токен обновления.</returns>
    [HttpPost]
    [ProducesResponseType<LoginResult>(StatusCodes.Status200OK)]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authorizationService.Login(dto.Login, dto.Password, Ct);
        HttpContext.Response.Cookies.Append("RefreshToken", result.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.MinutesRefreshTokenLifeTime)
            });

        return Ok(new LoginResult { AccessToken = result.AccessToken, ExpiresIn = _jwtSettings.MinutesLifeTime });
    }

    /// <summary>
    /// Обновить токен доступа.
    /// </summary>
    /// <returns>Токен обновления и токен доступа.</returns>
    [HttpPost]
    [ProducesResponseType<LoginResult>(StatusCodes.Status200OK)]
    [CookieRequired("RefreshToken", "Токен обновления")]
    [AuthorizedOnly(UserRoles = [])]
    public IActionResult RefreshToken()
    {
        var refreshToken = HttpContext.Request.Cookies["RefreshToken"];
        if (refreshToken is null)
        {
            return Unauthorized("Токен обновления пуст.");
        }

        var result = _authorizationService.RefreshToken(refreshToken);
        HttpContext.Response.Cookies.Append("RefreshToken", result.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.MinutesRefreshTokenLifeTime)
            });

        return Ok(new LoginResult { AccessToken = result.AccessToken, ExpiresIn = _jwtSettings.MinutesLifeTime });
    }
}