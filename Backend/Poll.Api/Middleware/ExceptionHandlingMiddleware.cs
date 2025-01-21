using Poll.Api.Models.Common;

namespace Poll.Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context, ILogger<ExceptionHandlingMiddleware> logger)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка обработки запроса {Ex}", ex.Message);
            context.Response.Clear();
            context.Response.StatusCode = 400;
            context.Response.ContentType = "text/json";
            await context.Response.WriteAsJsonAsync(new ApiErrorModel
            {
                Message = ex.Message,
                StatusCode = 400,
                TraceIdentifier = context.TraceIdentifier
            });
        }
    }
}