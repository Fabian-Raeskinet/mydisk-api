using Microsoft.Extensions.DependencyInjection;
using MyDisk.Infrastructure.Interfaces;
using MyDisk.Infrastructure.Persistence;

namespace MyDisk.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        return services;
    }
}