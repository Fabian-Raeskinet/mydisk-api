using Microsoft.Extensions.DependencyInjection;

namespace MyDisk.RetryService;

public static class DependencyInjection
{
    public static IServiceCollection AddRetryService(this IServiceCollection services)
    {
        return services.AddScoped<IRetryService, PollyRetryService>();
    }
}