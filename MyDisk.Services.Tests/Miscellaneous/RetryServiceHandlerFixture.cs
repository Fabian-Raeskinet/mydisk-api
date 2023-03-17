using FluentAssertions;
using Moq;
using MyDisk.Services.Miscellaneous;
using MyDisk.Tests.Services;
using MyDisk.Tests.Utils;

namespace MyDisk.Services.Tests.Miscellaneous;

public class RetryServiceHandlerFixture
{
    [Theory]
    [AutoServiceData]
    public async Task ShouldExecuteAsync
    (
        RetryServiceHandler sut,
        RetryServiceRequest request
    )
    {
        // Act
        await sut.Handle(request, default);

        // Assert
        sut.RetryService.AsMock()
            .Verify(x => x.ExecuteAsync(It.IsAny<Func<int, bool>>(), It.IsAny<Func<Task<int>>>()), Times.Once);

        sut.RetryService.AsMock()
            .Verify(x => x.ExecuteAsync<InvalidOperationException, int>(It.IsAny<Func<Task<int>>>()), Times.Once);
    }
}