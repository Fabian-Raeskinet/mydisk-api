using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyDisks.Contracts.Validators;
using MyDisks.Data;
using MyDisks.RandomServices;
using MyDisks.RetryService;
using MyDisks.Services;

namespace MyDisks.DependencyInjection.Configurations;

public static class sServiceCollectionExtensions
{
    public static void RegisterApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationServices();
        services.AddInfrastructureServices(configuration);
        services.AddContractValidators();
        services.AddRetryService();
        services.AddRandomServices();
    }
}