using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
    }
}