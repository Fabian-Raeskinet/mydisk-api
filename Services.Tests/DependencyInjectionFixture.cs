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
            var services = new ServiceCollection();

            // Act
            services.AddApplicationServices();
            var serviceProvider = services.BuildServiceProvider();

            // Assert
            var mediatr = serviceProvider.GetService<IMediator>();
            mediatr.Should().NotBeNull();

            var validationBehaviour =
                serviceProvider.GetService<IPipelineBehavior<GetDiskByNameQueryRequest, DiskResult>>();
            validationBehaviour.Should().NotBeNull();
            validationBehaviour.Should().BeOfType<ValidationBehaviour<GetDiskByNameQueryRequest, DiskResult>>();
        }
    }

    public class AddAutoMapperServicesFixture
    {
        [Fact]
        public void ShouldAddService()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddApplicationServices();
            var serviceProvider = services.BuildServiceProvider();

            // Assert
            var mapper = serviceProvider.GetService<IMapper>();
            mapper.Should().NotBeNull();
        }
    }

    public class ConfigureILoggerServicesFixture
    {
        [Fact]
        public void ShouldAddService()
        {
            // Arrange
            var services = new ServiceCollection();
            var loggerMock = new Mock<ILogger<GetAllDisksQueryHandler>>();
            services.AddSingleton(loggerMock.Object);

            // Act
            services.AddApplicationServices();
            var serviceProvider = services.BuildServiceProvider();

            // Assert
            var logger = serviceProvider.GetService<ILogger>();
            logger.Should().NotBeNull();
        }

        [Fact]
        public void ShouldNotAddService()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddApplicationServices();
            var serviceProvider = services.BuildServiceProvider();

            // Assert
            var logger = serviceProvider.GetService<ILogger>();
            logger.Should().BeNull();
        }
    }

    public class AddRetryServiceFixture
    {
        [Fact]
        public void ShouldAddService()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddApplicationServices();
            var serviceProvider = services.BuildServiceProvider();

            // Assert
            var logger = serviceProvider.GetService<IRetryService>();
            logger.Should().NotBeNull();
        }
    }
}