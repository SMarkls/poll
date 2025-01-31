using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Poll.Api.Models.Dto.Authorization;
using Poll.Api.Swagger.Attributes;
using Poll.Core.Configuration.Models;
using Poll.Core.Services.Authorization;

namespace Poll.Api.Controllers;

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

    [HttpPost]
    [ProducesResponseType<LoginResult>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authorizationService.Login(dto.Login, dto.Password, HttpContext.RequestAborted);
        HttpContext.Response.Cookies.Append("RefreshToken", result.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.MinutesRefreshTokenLifeTime)
            });

        return Ok(new LoginResult { AccessToken = result.AccessToken, ExpiresIn = _jwtSettings.MinutesLifeTime });
    }

    [HttpPost]
    [ProducesResponseType<LoginResult>(StatusCodes.Status200OK)]
    [CookieRequired("RefreshToken", "Токен обновления")]
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