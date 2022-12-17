using MyDisk.Api.Filters;

namespace MyDisk.Api;

public static class StartupMvc
{
    public static void AddApplicationMvc(this IServiceCollection services)
    {
        services.AddControllers()
            .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);
        services.AddMvc(options =>
        {
            options.Filters.Add<ApiExceptionFilterAttribute>();
        });
    }
}