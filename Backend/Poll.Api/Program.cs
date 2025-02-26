using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using NLog.Config;
using NLog.Extensions.Logging;
using Poll.Api.Extensions;
using Poll.Api.Filters;
using Poll.Api.Mappings;
using Poll.Api.Middleware;
using Poll.Api.Newtonsoft;
using Poll.Api.Swagger;
using Poll.Core;
using Poll.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services
    .AddControllers(opts =>
    {
        opts.Filters.Add<ValidationFilter>();
        opts.Filters.Add<AuthorizedOnlyAttribute>();
    })
    .ConfigureSerialization();

builder.Services
    .AddFluentValidationAutoValidation()
    .AddValidatorsFromAssemblyContaining<Program>()
    .RegisterConfiguration();

builder.Services.AddEndpointsApiExplorer();
builder.Services
    .AddSwagger()
    .AddSwaggerGen();

builder.Services.AddLogging(options =>
{
    options.ClearProviders();
    options.SetMinimumLevel(LogLevel.Trace);
    options.AddNLog();
    options.Configure(opts => opts.ActivityTrackingOptions = ActivityTrackingOptions.TraceId);
});

builder.Services
    .AddMappings()
    .AddCore(configuration)
    .AddInfrastructure(configuration);

builder.Services.Configure<ApiBehaviorOptions>(opts => opts.SuppressModelStateInvalidFilter = true);

var app = builder.Build();
app.UseCors(policy =>
{
    policy.AllowAnyMethod();
    policy.AllowAnyHeader();
    policy.AllowCredentials();
    policy.WithOrigins("http://localhost:5197");
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use(async (context, next) => {
    using (NLog.ScopeContext.PushProperty("TraceId", context.TraceIdentifier))
    {
        await next();
    }
});

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<UserSetMiddleware>();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();