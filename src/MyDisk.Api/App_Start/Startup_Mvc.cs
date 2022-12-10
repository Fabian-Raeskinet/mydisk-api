using MyDisk.Services.Common.Filters;
using MyDisk.Services.Filters;

namespace MyDisk.Api.App_Start
{
    public static class Startup_Mvc
    {
        public static IServiceCollection AddApplicationMvc(this IServiceCollection services, IConfiguration config)
        {
            services.AddControllers();
            services.AddMvc(options =>
            {
                options.Filters.Add<ServiceValidationExceptionFilter>();
                options.Filters.Add<ServiceEntityNotFoundExceptionFilter>();
            });
               

            return services;
        }
    }
}
