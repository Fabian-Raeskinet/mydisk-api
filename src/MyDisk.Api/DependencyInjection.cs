using FluentValidation.AspNetCore;
using MyDisk.Api.Filters;
using Newtonsoft.Json.Converters;

namespace MyDisk.Api;

public static class DependencyInjection
{
    public static void AddApiServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddApplicationMvc();
    }

    private static void AddApplicationMvc(this IServiceCollection services)
    {
        services.AddControllers()
            .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true)
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });
        services.AddMvc(options => { options.Filters.Add<ApiExceptionFilterAttribute>(); })
            .AddFluentValidation(x => x.AutomaticValidationEnabled = false);
    }
}