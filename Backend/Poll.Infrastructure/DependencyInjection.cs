using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Poll.Core.Configuration;
using Poll.Core.Configuration.Models;
using Poll.Core.Interfaces;
using Poll.Infrastructure.Ldap;
using Poll.Infrastructure.MongoConnection;
using Poll.Infrastructure.Repositories;

namespace Poll.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return AddMongoCollectionFactory(services).AddRepositories().RegisterConfiguration().AddLdap();
    }

    private static IServiceCollection AddMongoCollectionFactory(this IServiceCollection services)
    {
        return services.AddScoped(typeof(IMongoCollectionFactory<>), typeof(MongoCollectionFactory<>));
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services.AddTransient(typeof(IRepository<Core.Entities.Poll>), typeof(PollRepository));
    }

    private static IServiceCollection AddLdap(this IServiceCollection services)
    {
        return services.AddScoped<ILdapService, LdapService>();
    }
}