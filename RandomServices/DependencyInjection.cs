using Microsoft.Extensions.DependencyInjection;

namespace MyDisk.RandomServices;

public static class DependencyInjection
{
    public static IServiceCollection AddRandomServices(this IServiceCollection services)
    {
        return services.AddScoped<IRandomService, RandomService>();
    }
}