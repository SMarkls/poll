using System.Security.Claims;
using Poll.Core.Services.Authorization;

namespace Poll.Api.Middleware;

public class UserSetMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context, IAuthorizationService authorizationService)
    {
        var token = context.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        if (!string.IsNullOrEmpty(token) && authorizationService.ValidateToken(token, out var id, out var login, out var role))
        {
            context.User = new ClaimsPrincipal(new List<ClaimsIdentity>
            {
                new(new List<Claim>
                {
                    new(Core.Consts.Authorization.Claims.IdentifierClaimType, id),
                    new(Core.Consts.Authorization.Claims.LoginClaimType, login),
                    new(Core.Consts.Authorization.Claims.RoleClaimType, role.ToString())
                })
            });
        }

        await next(context);
    }
}