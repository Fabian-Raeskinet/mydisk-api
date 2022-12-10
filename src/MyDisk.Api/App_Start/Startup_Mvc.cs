using MyDisk.Api.Filters;

namespace MyDisk.Api.App_Start
{
    public static class Startup_Mvc
    {
        public static IServiceCollection AddApplicationMvc(this IServiceCollection services, IConfiguration config)
        {
            services.AddControllers();
            services.AddMvc(options =>
            {
                options.Filters.Add<ApiExceptionFilterAttribute>();
            });
               
            return services;
        }
    }
}
