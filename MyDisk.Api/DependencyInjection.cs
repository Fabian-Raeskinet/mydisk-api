using FluentValidation.AspNetCore;
using MyDisk.Api.Filters;
using MyDisk.Api.Resiliences;
using MyDisk.RetryService;
using Newtonsoft.Json.Converters;
using Polly.Registry;

namespace MyDisk.Api;

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
        services.AddMvc(options => { options.Filters.Add<ApiExceptionFilterAttribute>(); })
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

    private static void AddOptionSettings(this IServiceCollection services, IConfiguration configuration) =>
        services.Configure<RetryServiceSettings>(configuration.GetSection(nameof(RetryServiceSettings)));

    private static void AddPolicyRegistry(this IServiceCollection services)
    {
        var httpRetryPolicy = RetryPolicy.Create<HttpResponse>(x => x.StatusCode != 200, 3, 1000);
        var registry = new PolicyRegistry()
        {
            {"HttpRetryPolicy", httpRetryPolicy}
        };

        services.AddSingleton<IReadOnlyPolicyRegistry<string>>(registry);
    }
}