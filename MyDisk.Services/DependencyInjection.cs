﻿using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyDisk.RetryService;
using MyDisk.Services.Common.Behaviors;
using MyDisk.Services.Disks.Queries;
using MyDisk.Services.Disks.Requests;

namespace MyDisk.Services;

public static class DependencyInjection
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services
            .AddMediatRServices()
            .AddAutoMapperServices()
            .ConfigureILoggerServices()
            .AddRetryService();
    }

    private static IServiceCollection AddAutoMapperServices(this IServiceCollection services)
    {
        return services.AddAutoMapper(Assembly.GetExecutingAssembly());
    }

    private static IServiceCollection AddMediatRServices(this IServiceCollection services)
    {
        services.AddMediatR(typeof(GetAllDisksQueryHandler).Assembly);
        services.AddValidatorsFromAssemblyContaining<GetDiskByNameRequestValidator>();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

        return services;
    }

    private static IServiceCollection ConfigureILoggerServices(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetService<ILogger<GetAllDisksQueryHandler>>();
        if (logger != null) services.AddSingleton(typeof(ILogger), logger);

        return services;
    }

    private static IServiceCollection AddRetryService(this IServiceCollection services)
    {
        return services.AddScoped<IRetryService, PollyRetryService>();
    }
}