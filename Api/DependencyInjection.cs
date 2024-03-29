﻿using FluentValidation.AspNetCore;
using MyDisks.Api.Filters;
using MyDisks.Api.Maintenance;
using MyDisks.Api.Resiliences;
using MyDisks.RetryService;
using Newtonsoft.Json.Converters;
using Polly.Registry;

namespace MyDisks.Api;

public static class DependencyInjection
{
    public static void AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddMvcConfiguration();
        services.AddControllerConfiguration();
        services.AddSwaggerConfiguration();
        services.AddOptionSettings(configuration);
        services.AddPolicyRegistry();
    }

    private static void AddMvcConfiguration(this IServiceCollection services)
    {
        services
            .AddMvc(options =>
            {
                options.Filters.Add<ApiExceptionFilterAttribute>();
                options.Filters.Add<MaintenanceFilterAttribute>();
            })
            .AddFluentValidation(x => x.AutomaticValidationEnabled = false);
    }

    private static void AddControllerConfiguration(this IServiceCollection services)
    {
        services.AddControllers()
            .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true)
            .AddNewtonsoftJson(options => { options.SerializerSettings.Converters.Add(new StringEnumConverter()); });
    }

    private static void AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddSwaggerGen();
        services.AddSwaggerGenNewtonsoftSupport();
    }

    private static void AddOptionSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RetryServiceSettings>(configuration.GetSection(nameof(RetryServiceSettings)));
        services.Configure<MaintenanceSettings>(configuration.GetSection(nameof(MaintenanceSettings)));
    }

    private static void AddPolicyRegistry(this IServiceCollection services)
    {
        var httpRetryPolicy = RetryPolicy.Create<HttpResponse>(x => x.StatusCode != 200, 3, 1000);
        var registry = new PolicyRegistry
        {
            { "HttpRetryPolicy", httpRetryPolicy }
        };

        services.AddSingleton<IReadOnlyPolicyRegistry<string>>(registry);
    }
}