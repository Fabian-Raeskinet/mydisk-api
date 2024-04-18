using Moq;
using MyDisks.Domain;
using MyDisks.Domain.Disks;
using MyDisks.Tests.Services;
using MyDisks.Tests.Utils;

namespace MyDisks.Services.Tests;

public class MediatRDomainEventDispatcherFixture
{
    public class DispatchFixture
    {
        [Theory]
        [AutoServiceData]
        public async Task ShouldDispatch(IDomainEvent domainEvent, MediatRDomainEventDispatcher sut)
        {
            // Act
            await sut.Dispatch(domainEvent, CancellationToken.None);
            
            // Assert
            sut.Mediator.AsMock()
                .Verify(x => x.Publish(domainEvent, CancellationToken.None));
        }
    }
}