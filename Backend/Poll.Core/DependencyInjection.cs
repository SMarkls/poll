using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Poll.Core.Services.Authorization;
using Poll.Core.Services.Poll;

namespace Poll.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
    {
        return AddServices(services);
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services
            .AddTransient<IPollService, PollService>()
            .AddTransient<IAuthorizationService, AuthorizationService>();
    }
}