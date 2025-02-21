using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Poll.Api.Models.Common;

namespace Poll.Api.Filters;

public class ValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState.Where(x => x.Value is not null).SelectMany(x => x.Value!.Errors);
            var message = string.Join(";\n", errors.Select(x => x.ErrorMessage));
            context.Result = new BadRequestObjectResult(new ApiErrorModel
            {
                Message = message, StatusCode = 400, TraceIdentifier = context.HttpContext.TraceIdentifier
            });
            return;
        }

        await next();
    }
}