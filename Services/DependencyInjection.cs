using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyDisks.Domain;
using MyDisks.RandomServices;
using MyDisks.RetryService;
using MyDisks.Services.Behaviors;
using MyDisks.Services.Disks;

namespace MyDisks.Services;

public static class DependencyInjection
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services
            .AddMediatRServices()
            .AddAutoMapperServices()
            .ConfigureILoggerServices();
    }

    private static IServiceCollection AddAutoMapperServices(this IServiceCollection services)
    {
        return services.AddAutoMapper(Assembly.GetExecutingAssembly());
    }

    private static IServiceCollection AddMediatRServices(this IServiceCollection services)
    {
        services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();

        return services;
    }

    private static IServiceCollection ConfigureILoggerServices(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetService<ILogger<GetAllDisksQueryHandler>>();
        if (logger != null) services.AddSingleton(typeof(ILogger), logger);

        return services;
    }
}