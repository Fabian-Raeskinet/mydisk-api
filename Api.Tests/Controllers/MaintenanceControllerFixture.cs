using MyDisks.Api.Maintenance;
using MyDisks.Tests.Api;
using MyDisks.Tests.Utils;

namespace MyDisks.Api.Tests.Controllers;

public class MaintenanceControllerFixture
{
    public class IsMaintenanceFixture
    {
        [Theory]
        [AutoApiData]
        public void Should_Be_Active([NoAutoProperties] MaintenanceController sut)
        {
            // Arrange
            sut.MaintenanceSettings.AsMock()
                .SetupGet(x => x.Value).Returns(new MaintenanceSettings { Global = true });

            // Act
            var act = sut.IsMaintenance();

            // Assert
            act.As<OkObjectResult>().Value.As<MaintenanceStatusResult>().Status.Should().Be(MaintenanceStatus.Active);
        }

        [Theory]
        [AutoApiData]
        public void Should_Be_Inactive([NoAutoProperties] MaintenanceController sut)
        {
            // Arrange
            sut.MaintenanceSettings.AsMock()
                .SetupGet(x => x.Value)
                .Returns(new MaintenanceSettings { Global = false });

            // Act
            var act = sut.IsMaintenance();

            // Assert
            act.As<OkObjectResult>().Value.As<MaintenanceStatusResult>().Status.Should().Be(MaintenanceStatus.Inactive);
        }

        [Theory]
        [AutoApiData]
        public void Should_Returns_Ok_Object_Result([NoAutoProperties] MaintenanceController sut)
        {
            // Arrange
            sut.MaintenanceSettings.AsMock()
                .SetupGet(x => x.Value)
                .Returns(new MaintenanceSettings { Global = true });

            // Act
            var act = sut.IsMaintenance();

            // Assert
            act.Should().BeOfType<OkObjectResult>();
        }
    }
}