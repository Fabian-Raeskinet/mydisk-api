using MyDisks.Services.Miscellaneous;
using MyDisks.Tests.Api;
using MyDisks.Tests.Utils;

namespace MyDisks.Api.Tests.Controllers;

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