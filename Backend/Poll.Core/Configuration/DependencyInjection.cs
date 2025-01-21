using Microsoft.Extensions.DependencyInjection;
using Poll.Core.Configuration.Models;

namespace Poll.Core.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection RegisterConfiguration(this IServiceCollection services)
    {
        return services
            .RegisterOptionsWithValidation<MongoConfiguration>()
            .RegisterOptionsWithValidation<LdapSettings>()
            .RegisterOptionsWithValidation<JwtSettings>();
    }

    private static IServiceCollection RegisterOptionsWithValidation<T>(this IServiceCollection services) 
        where T: class
    {
        services.AddOptions<T>().BindConfiguration(typeof(T).Name).ValidateDataAnnotations().ValidateOnStart()
            .Validate(x => x != null);
        return services;
    }
}