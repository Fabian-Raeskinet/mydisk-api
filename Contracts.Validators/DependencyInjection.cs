using Contracts.Validators.Disks;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Contracts.Validators;

public static class DependencyInjection
{
    private static IServiceCollection AddFluentValidationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<GetDiskByNameQueryValidator>();

        return services;
    }

    public static void AddContractValidators(this IServiceCollection services)
    {
        services.AddFluentValidationServices();
    }
}