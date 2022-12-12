using Microsoft.Extensions.DependencyInjection;
using MyDisk.Infrastructure.Persistence;
using MyDisk.Services.Common.Interfaces;

namespace MyDisk.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

            return services;
        }
    }
}
