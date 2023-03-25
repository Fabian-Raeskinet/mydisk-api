using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MediatorExtension;

public static class DependencyInjection
{
    
    public static void AddMediatorExtension(this IServiceCollection services)
    {
        services.AddMediatRExtension();
    }
    
    private static IServiceCollection AddMediatRExtension(this IServiceCollection services)
    {
        services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        return services;
    }
}