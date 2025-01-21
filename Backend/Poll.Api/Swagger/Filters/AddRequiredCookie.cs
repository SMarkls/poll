using Microsoft.OpenApi.Models;
using Poll.Api.Swagger.Attributes;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Poll.Api.Swagger.Filters;

public class AddRequiredCookie : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var cookies = context.MethodInfo.GetCustomAttributes(true)
            .OfType<CookieRequiredAttribute>();

        operation.Parameters ??= new List<OpenApiParameter>();
        foreach (CookieRequiredAttribute cookie in cookies)
        {
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = cookie.CookieName,
                Description = cookie.Description,
                Required = true,
                In = ParameterLocation.Cookie
            });
        }
    }
}
