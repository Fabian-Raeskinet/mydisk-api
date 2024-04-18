using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyDisks.Domain;
using MyDisks.Domain.Authors;
using MyDisks.Domain.Disks;
using MyDisks.Domain.Reviews;

namespace MyDisks.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .AddRepositories()
            .AddDatabaseConfiguration(configuration)
            .AddDateTimeService();
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<IDiskRepository, DiskRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();

        return services;
    }

    private static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
       // services.AddDbContext<ApplicationDbContext>(options =>
         //   options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

         services.AddDbContext<ApplicationDbContext>(options =>
              options.UseInMemoryDatabase("DbTest"));

        services.AddScoped<ApplicationDbContextInitializer>();
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<EntitySaveChangesInterceptor>();

        return services;
    }

    private static IServiceCollection AddDateTimeService(this IServiceCollection services)
    {
        return services.AddScoped<IDateTime, DateTimeService>();
    }
}