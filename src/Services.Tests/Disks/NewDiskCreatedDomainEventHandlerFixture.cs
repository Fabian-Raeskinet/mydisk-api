using Microsoft.Extensions.Logging;
using Moq;
using MyDisks.Domain.Disks;
using MyDisks.Services.Disks;
using MyDisks.Tests.Services;
using MyDisks.Tests.Utils;

namespace MyDisks.Services.Tests.Disks;

public class NewDiskCreatedDomainEventHandlerFixture
{
    public class HandleFixture
    {
        [Theory]
        [AutoServiceData]
        public async Task Should_Log(NewDiskCreatedDomainEvent domainEvent, NewDiskCreatedDomainEventHandler sut)
        {
            // Act
            await sut.Handle(domainEvent, CancellationToken.None);
            
            // Assert
            sut.Logger.AsMock()
                .Verify(logger => logger.Log(
                        It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                        It.Is<EventId>(eventId => eventId.Id == 0),
                        It.IsAny<It.IsAnyType>(),
                        It.IsAny<Exception>(),
                        It.IsAny<Func<It.IsAnyType, Exception, string>>()!) );
        }
    }
}