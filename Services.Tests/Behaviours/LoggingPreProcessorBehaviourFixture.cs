using Microsoft.Extensions.Logging;
using Moq;
using MyDisks.Services.Behaviors;
using MyDisks.Services.Disks;
using MyDisks.Tests.Services;
using MyDisks.Tests.Utils;

namespace MyDisks.Services.Tests.Behaviours;

public class LoggingPreProcessorBehaviourFixture
{
    [Theory]
    [AutoServiceData]
    public async Task ShouldLog
    (
        LoggingPreProcessorBehaviour<GetDiskByNameQueryRequest> sut,
        GetDiskByNameQueryRequest request
    )
    {
        // Act
        await sut.Process(request, default);

        //Assert
        sut.Logger.AsMock()
            .Verify(logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
                Times.Once);
    }
}