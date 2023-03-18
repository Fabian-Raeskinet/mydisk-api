using MyDisk.Services.Miscellaneous;
using MyDisk.Tests.Api;
using MyDisk.Tests.Utils;

namespace MyDisk.Api.Tests.Controllers;

public class MiscellaneousControllerFixture
{
    public class RetryServiceShould
    {
        [Theory]
        [AutoApiData]
        public async void ReturnsNoContentResult([NoAutoProperties] MiscellaneousController sut)
        {
            // Arrange
            sut.Mediator.AsMock()
                .Setup(x => x.Send(It.IsAny<RetryServiceRequest>(), default))
                .Verifiable();
            // Act
            var act = await sut.RetryServiceAction();

            // Assert
            act.Should().BeOfType<NoContentResult>();
            sut.Mediator.AsMock()
                .Verify(x => x.Send(It.IsAny<RetryServiceRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }

}