using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace MyDisks.Messaging;

public static class DependencyInjection
{
    public static IServiceCollection AddRabbitMqMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(RabbitMqOptions.SectionName);
        if (string.IsNullOrWhiteSpace(section[nameof(RabbitMqOptions.HostName)]))
            return services;

        services.AddOptions<RabbitMqOptions>()
            .Bind(section)
            .Validate(o => !string.IsNullOrWhiteSpace(o.UserName) && !string.IsNullOrEmpty(o.Password),
                "RabbitMq:UserName and RabbitMq:Password are required.")
            .Validate(o => o.Port is > 0 and <= 65535, "RabbitMq:Port must be between 1 and 65535.")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Exchange), "RabbitMq:Exchange is required.")
            .ValidateOnStart();

        services.AddSingleton(provider =>
        {
            var options = provider.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
            return new ConnectionFactory
            {
                HostName = options.HostName,
                Port = options.Port,
                VirtualHost = options.VirtualHost,
                UserName = options.UserName,
                Password = options.Password,
                ClientProvidedName = "mydisk-api",
                AutomaticRecoveryEnabled = false
            };
        });
        services.AddHostedService<RabbitMqOutboxPublisher>();

        return services;
    }
}
