using Microsoft.AspNetCore.Mvc.Filters;

namespace Poll.Api.Swagger.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public class CookieRequiredAttribute : Attribute
{
    public string CookieName { get; }
    public string? Description { get; }

    public CookieRequiredAttribute(string cookieName, string? description = null)
    {
        CookieName = cookieName;
        Description = description;
    }
}