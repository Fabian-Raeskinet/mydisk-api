using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MyDisks.Contracts.Validators.Disks;

namespace MyDisks.Contracts.Validators;

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