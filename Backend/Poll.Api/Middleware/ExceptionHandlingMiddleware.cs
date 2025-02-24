using Poll.Api.Models.Common;
using Poll.Core.Exceptions;

namespace Poll.Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context, ILogger<ExceptionHandlingMiddleware> logger)
    {
        try
        {
            await next(context);
        }
        catch (AppException ex)
        {
            logger.LogError(ex, "Ошибка обработки запроса {Ex}", ex.Message);
            context.Response.Clear();
            context.Response.StatusCode = ex.StatusCode;
            context.Response.ContentType = "text/json";
            await context.Response.WriteAsJsonAsync(new ApiErrorModel
            {
                Message = ex.Message,
                StatusCode = ex.StatusCode,
                TraceIdentifier = context.TraceIdentifier
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка обработки запроса {Ex}", ex.Message);
            context.Response.Clear();
            context.Response.StatusCode = 500;
            context.Response.ContentType = "text/json";
            await context.Response.WriteAsJsonAsync(new ApiErrorModel
            {
                Message = ex.Message,
                StatusCode = 500,
                TraceIdentifier = context.TraceIdentifier
            });
        }
    }
}