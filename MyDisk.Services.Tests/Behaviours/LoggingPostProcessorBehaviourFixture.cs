using Microsoft.Extensions.Logging;
using Moq;
using MyDisk.Contracts.Disks;
using MyDisk.Services.Behaviors;
using MyDisk.Services.Disks;
using MyDisk.Tests.Services;
using MyDisk.Tests.Utils;

namespace MyDisk.Services.Tests.Behaviours;

public class LoggingPostProcessorBehaviourFixture
{
    [Theory]
    [AutoServiceData]
    public async Task ShouldLog
    (
        LoggingPostProcessorBehaviour<GetDiskByNameQueryRequest, DiskResult> sut,
        GetDiskByNameQueryRequest request,
        DiskResult response
    )
    {
        // Act
        await sut.Process(request, response, default);

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