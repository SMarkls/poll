using Microsoft.AspNetCore.Mvc;
using Poll.Api.Models.Common;
using Poll.Core.Consts.Authorization;

namespace Poll.Api.Controllers;

/// <summary>
/// Абстрактный контроллер для общей логики.
/// </summary>
[ApiController]
[Route("[controller]")]
[ProducesResponseType(typeof(ApiErrorModel), StatusCodes.Status400BadRequest)]
public abstract class BaseController : Controller
{
    protected CancellationToken Ct => HttpContext.RequestAborted;
    private CurrentUser? _currentUser;
    public CurrentUser CurrentUser => _currentUser ??= GetCurrentUser();
    private CurrentUser GetCurrentUser()
    {
        var user = HttpContext.User;
        var id = user.Claims.FirstOrDefault(x => x.Type == Claims.IdentifierClaimType)?.Value;
        var login = user.Claims.FirstOrDefault(x => x.Type == Claims.LoginClaimType)?.Value;
        var role = user.Claims.FirstOrDefault(x => x.Type == Claims.RoleClaimType)?.Value;
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(login) || string.IsNullOrEmpty(role))
        {
            throw new Exception("Пользователь не авторизован");
        }

        return new CurrentUser { Id = id, Login = login, Role = role };
    }
}