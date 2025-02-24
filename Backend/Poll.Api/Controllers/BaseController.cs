using Microsoft.AspNetCore.Mvc;
using Poll.Api.Models.Common;
using Poll.Core.Consts.Authorization;
using Poll.Core.Exceptions;

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
            throw new PermissionDeniedException();
        }

        return new CurrentUser { Id = id, Login = login, Role = role };
    }

    protected IActionResult Error(ApiErrorModel error)
    {
        var method = MethodFactory(this, error.StatusCode);
        return method(error);
    }

    protected IActionResult Error(string message, int statusCode)
    {
        return Error(new ApiErrorModel
        {
            Message = message, StatusCode = statusCode, TraceIdentifier = HttpContext.TraceIdentifier
        });
    }

    private static Func<object, IActionResult> MethodFactory(ControllerBase controller, int statusCode)
    {
        return statusCode switch
        {
            404 => controller.NotFound,
            400 => controller.BadRequest,
            401 => controller.Unauthorized,
            409 => controller.Conflict,
            422 => controller.UnprocessableEntity,
            _ => controller.BadRequest
        };
    }

}