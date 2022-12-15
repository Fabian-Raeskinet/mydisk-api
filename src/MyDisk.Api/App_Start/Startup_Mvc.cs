using MyDisk.Api.Filters;

namespace MyDisk.Api;

public static class StartupMvc
{
    public static void AddApplicationMvc(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddMvc(options =>
        {
            options.Filters.Add<ApiExceptionFilterAttribute>();
        });
    }
}