using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Poll.Core.Entities.Ldap;

namespace Poll.Api.Filters;

/// <summary>
/// Аттрибут авторзиации пользователя.
/// Пустой массив ролей допускает любого пользователя.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
public class AuthorizedOnlyAttribute : Attribute, IAuthorizationFilter
{
    public required UserRole[] UserRoles { get; init; }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
        if (allowAnonymous)
        {
            return;
        }

        var roleValue = context.HttpContext.User.Claims
            .FirstOrDefault(x => x.Type == Core.Consts.Authorization.Claims.RoleClaimType)?
            .Value;
        if (!Enum.TryParse(roleValue, out UserRole role))
        {
            throw new Exception("Пользователь не авторизован");
        }
        
        if (UserRoles.Length != 0 && !UserRoles.Contains(role))
        {
            throw new Exception("Пользователь не имеет доступа к этому ресурсу");
        }
    }
}