using FluentValidation;
using MediatR;
using MyDisk.Services.Common.Behaviors;
using MyDisk.Services.Disks.Queries;
using MyDisk.Services.Disks.Requests;

namespace MyDisk.Api.App_Start
{
    public static class Startup_Services
    {

        public static IServiceCollection AddApplicationServices(this IServiceCollection services) =>
            services
            .AddMediatRHandlers()
            .ConfigureILogger();

        private static IServiceCollection AddMediatRHandlers(this IServiceCollection services)
        {
            services.AddMediatR(typeof(GetAllDisksQueryHandler).Assembly);
            services.AddValidatorsFromAssemblyContaining<GetDiskByNameRequestValidator>();
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidatorPipelineBehavior<,>));



            return services;
        }

        private static IServiceCollection ConfigureILogger(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetService<ILogger<GetAllDisksQueryHandler>>();
            if (logger != null)
            {
                services.AddSingleton(typeof(ILogger), logger);
            }

            return services;
        }
    }
}
