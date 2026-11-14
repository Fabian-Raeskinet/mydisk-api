using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using MyDisks.RetryService;
using MyDisks.Tests.Api;
using Polly.Registry;
using Swashbuckle.AspNetCore.Swagger;

namespace MyDisks.Api.Tests;

public class DependencyInjectionFixture
{
    public class AddSwaggerConfigurationFixture
    {
        [Theory]
        [AutoApiData]
        public void Should_Add_AddSwaggerConfiguration(IConfiguration configuration)
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddApiServices(configuration);

            // Act
            var serviceProvider = services.BuildServiceProvider();

            // Assert
            var swaggerGenServiceDescriptor = services
                .FirstOrDefault(descriptor => descriptor.ServiceType == typeof(ISwaggerProvider));
            swaggerGenServiceDescriptor.Should().NotBeNull();
        }

        [Theory]
        [AutoApiData]
        public void Should_Add_AddPolicyRegistry(IConfiguration configuration)
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddApiServices(configuration);
            var serviceProvider = services.BuildServiceProvider();

            // Assert
            var policyRegistry = serviceProvider.GetRequiredService<IReadOnlyPolicyRegistry<string>>();
            policyRegistry.Should().NotBeNull();
        }

        [Theory]
        [AutoApiData]
        public void Should_Add_AddOptionSettings(IConfiguration configuration)
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddApiServices(configuration);
            var serviceProvider = services.BuildServiceProvider();

            // Assert
            var retryServiceSettings = serviceProvider.GetRequiredService<IOptions<RetryServiceSettings>>().Value;
            retryServiceSettings.Should().NotBeNull();
        }

        [Theory]
        [AutoApiData]
        public void Should_Add_HealthChecks(IConfiguration configuration)
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddApiServices(configuration);
            var serviceProvider = services.BuildServiceProvider();

            // Assert
            var healthCheckService = serviceProvider.GetService<HealthCheckService>();
            healthCheckService.Should().NotBeNull();
        }

        [Fact]
        public void Should_Add_Postgres_HealthCheck_When_ConnectionString_Exists()
        {
            // Arrange
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "ConnectionStrings:DefaultConnection", "Host=localhost;Database=test;Username=user" }
                })
                .Build();
            var services = new ServiceCollection();

            // Act
            services.AddApiServices(configuration);
            var serviceProvider = services.BuildServiceProvider();

            // Assert
            var options = serviceProvider.GetRequiredService<IOptions<HealthCheckServiceOptions>>().Value;
            var registration = options.Registrations.Single(r => r.Name == "npgsql");
            registration.Tags.Should().Contain("ready");
            registration.Timeout.Should().Be(TimeSpan.FromSeconds(2));
        }
    }
}