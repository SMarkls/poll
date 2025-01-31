using System.Reflection;
using Microsoft.OpenApi.Models;
using Poll.Api.Swagger.Filters;
using Poll.Core.Entities.Variants;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Poll.Api.Swagger;

public static class SwaggerExtension
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.OperationFilter<AddRequiredCookie>();
            c.SwaggerDoc("v1", new OpenApiInfo { Version = "v1", Title = "API для сервиса опросов" });
            c.DescribeAllParametersInCamelCase();

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description =
                    "Авторизация на основе JWT, используя схему Bearer. \r\n\r\n" +
                    "Введите 'Bearer' [space] и вставьте свой токен.\r\n\r\n" +
                    "Например: \"Bearer eydslkfjhuh.\"",
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    []
                }
            });
        });

        return services;
    }
}