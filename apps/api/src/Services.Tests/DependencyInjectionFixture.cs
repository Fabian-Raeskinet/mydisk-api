using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using MyDisks.Contracts.Disks;
using MyDisks.RetryService;
using MyDisks.Services.Behaviors;
using MyDisks.Services.Disks;

namespace MyDisks.Services.Tests;

public class DependencyInjectionFixture
{
    public class AddMediatRServicesFixture
    {
        [Fact]
        public void ShouldAddService()
        {
            // Arrange
            var services = CreateServices();

            // Act
            services.AddApplicationServices();
            var serviceProvider = services.BuildServiceProvider();

            // Assert
            var mediatr = serviceProvider.GetService<IMediator>();
            mediatr.Should().NotBeNull();

            var behaviours = serviceProvider.GetServices<IPipelineBehavior<GetDiskByNameQueryRequest, DiskResult>>();
            behaviours.Should().ContainSingle(b => b is ValidationBehaviour<GetDiskByNameQueryRequest, DiskResult>);
        }
    }

    public class AddAutoMapperServicesFixture
    {
        [Fact]
        public void ShouldAddService()
        {
            // Arrange
            var services = CreateServices();

            // Act
            services.AddApplicationServices();
            var serviceProvider = services.BuildServiceProvider();

            // Assert
            var mapper = serviceProvider.GetService<IMapper>();
            mapper.Should().NotBeNull();
        }
    }

    private static ServiceCollection CreateServices()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddScoped(_ => Mock.Of<IUnitOfWork>());
        return services;
    }

    // public class AddRetryServiceFixture
    // {
    //     [Fact]
    //     public void ShouldAddService()
    //     {
    //         // Arrange
    //         var services = new ServiceCollection();
    //
    //         // Act
    //         services.AddApplicationServices();
    //         var serviceProvider = services.BuildServiceProvider();
    //
    //         // Assert
    //         var logger = serviceProvider.GetService<IRetryService>();
    //         logger.Should().NotBeNull();
    //     }
    // }
}