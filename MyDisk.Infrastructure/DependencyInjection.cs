using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyDisk.Domain.Interfaces;
using MyDisk.Domain.Interfaces.IRepositories;
using MyDisk.Infrastructure.Interfaces;
using MyDisk.Infrastructure.Persistence;
using MyDisk.Infrastructure.Persistence.Identity;
using MyDisk.Infrastructure.Persistence.Interceptors;
using MyDisk.Infrastructure.Repositories;
using MyDisk.Infrastructure.Services;

namespace MyDisk.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration) =>
        services
            .AddRepositories()
            .AddDatabaseConfiguration(configuration)
            .AddDateTimeService()
            .AddIdentityService();

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<IDiskRepository, DiskRepository>();
        return services;
    }

    private static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ApplicationDbContextInitializer>();
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<EntitySaveChangesInterceptor>();

        return services;
    }

    private static IServiceCollection AddDateTimeService(this IServiceCollection services) =>
        services.AddTransient<IDateTime, DateTimeService>();

    private static IServiceCollection AddIdentityService(this IServiceCollection services)
    {
        services
            .AddDefaultIdentity<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddIdentityServer()
            .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

        services.AddTransient<IIdentityService, IdentityService>();

        return services;
    }
}