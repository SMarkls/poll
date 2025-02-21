using FluentValidation;
using Microsoft.Extensions.Options;
using Poll.Api.Models.Validation;
using Poll.Core.Configuration.Models;

namespace Poll.Api.Extensions;

public static class ConfigurationExtension
{
    public static IServiceCollection RegisterConfiguration(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssemblyOf<Program>()
            .AddClasses(classes => classes.AssignableTo(typeof(AbstractValidator<>)))
            .AsImplementedInterfaces()
            .WithSingletonLifetime()
        );

        return services
#if DEBUG
            .RegisterOptions<TestingSettings>()
#endif
            .RegisterOptionsWithValidation<MongoConfiguration>()
            .RegisterOptionsWithValidation<LdapSettings>()
            .RegisterOptionsWithValidation<JwtSettings>();
    }

    private static IServiceCollection RegisterOptionsWithValidation<T>(
        this IServiceCollection services) where T : class
    {
        services.AddOptions<T>()
            .BindConfiguration(typeof(T).Name)
            .ValidateFluentValidation()
            .ValidateOnStart();

        return services;
    }

    private static IServiceCollection RegisterOptions<T>(this IServiceCollection services) where T : class
    {
        services.AddOptions<T>().BindConfiguration(typeof(T).Name);
        return services;
    }

    private static OptionsBuilder<T> ValidateFluentValidation<T>(
        this OptionsBuilder<T> optionsBuilder) where T : class
    {
        optionsBuilder.Services.AddSingleton<IValidateOptions<T>>(provider =>
            new FluentValidationOptions<T>(provider.GetService<IValidator<T>>()));
    
        return optionsBuilder;
    }
}