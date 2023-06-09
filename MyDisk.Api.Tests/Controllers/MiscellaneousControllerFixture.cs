using MyDisk.Services.Miscellaneous;
using MyDisk.Tests.Api;
using MyDisk.Tests.Utils;

namespace MyDisk.Api.Tests.Controllers;

public class MiscellaneousControllerFixture
{
    public class RetryServiceActionFixture
    {
        [Theory]
        [AutoApiData]
        public async void Should_Use_Mediator([NoAutoProperties] MiscellaneousController sut)
        {
            // Act
            var act = await sut.RetryServiceAction();

            // Assert
            sut.Mediator.AsMock()
                .Verify(x => x.Send(It.IsAny<RetryServiceRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        
        [Theory]
        [AutoApiData]
        public async void Should_Returns_NoContentResult([NoAutoProperties] MiscellaneousController sut)
        {
            // Act
            var act = await sut.RetryServiceAction();

            // Assert
            act.Should().BeOfType<NoContentResult>();
        }
    }
}