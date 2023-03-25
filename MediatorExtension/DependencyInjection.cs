using MediatorExtension.Disks;
using MediatR;
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
        services.AddMediatR(typeof(GetDiskByNameQueryRequest).Assembly);

        return services;
    }
}