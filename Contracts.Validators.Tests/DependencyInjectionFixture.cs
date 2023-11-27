using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MyDisks.RetryService;
using MyDisks.Services.Disks;

namespace MyDisks.Contracts.Validators.Tests;

public class DependencyInjectionFixture
{
    public class AddFluentValidationServicesFixture
    {
        [Fact]
        public void ShouldAddService()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddContractValidators();
            var serviceProvider = services.BuildServiceProvider();

            // Assert
            var validator = serviceProvider.GetService<IValidator<GetDiskByNameQueryRequest>>();
            validator.Should().NotBeNull();
        }
    }
}