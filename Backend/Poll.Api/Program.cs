using Microsoft.AspNetCore.Mvc;
using NLog.Extensions.Logging;
using Poll.Api.Mappings;
using Poll.Api.Middleware;
using Poll.Api.Swagger;
using Poll.Core;
using Poll.Core.Configuration;
using Poll.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging(options =>
{
    options.ClearProviders();
    options.SetMinimumLevel(LogLevel.Trace);
    options.AddNLog();
});

builder.Services
    .AddSwagger()
    .AddMappings()
    .AddCore(configuration)
    .AddInfrastructure(configuration);

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

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<UserSetMiddleware>();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
