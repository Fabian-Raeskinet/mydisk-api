using Microsoft.Extensions.DependencyInjection;
using MyDisk.Infrastructure.Interfaces;
using MyDisk.Infrastructure.Interfaces.IRepositories;
using MyDisk.Infrastructure.Persistence;
using MyDisk.Infrastructure.Repositories;

namespace MyDisk.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services) => 
        services.AddRepositories();
    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<IDiskRepository, DiskRepository>();
        return services;
    }
}