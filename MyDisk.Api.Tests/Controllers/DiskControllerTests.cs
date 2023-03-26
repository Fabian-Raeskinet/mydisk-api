using MyDisk.Contracts.Disks;
using MyDisk.Services.Disks;
using MyDisk.Tests.Api;
using MyDisk.Tests.Utils;

namespace MyDisk.Api.Tests.Controllers;

public class GetAllDisksShould
{
    [Theory]
    [AutoApiData]
    public async void ReturnsOkResultTest([NoAutoProperties] DiskController sut)
    {
        // Arrange
        sut.Mediator.AsMock()
            .Setup(x => x.Send(It.IsAny<GetAllDisksQueryRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DiskResponse>());
        // Act
        var act = await sut.GetAllDisks();

        // Assert
        act.Should().BeOfType<OkObjectResult>();
        sut.Mediator.AsMock()
            .Verify(
                x => x.Send(It.IsAny<GetAllDisksQueryRequest>(), It.IsAny<CancellationToken>()),
                Times.Once);
    }
}

public class GetDiskByNameShould
{
    [Theory]
    [AutoApiData]
    public async void ReturnsOkResult([NoAutoProperties] DiskController sut, DiskResponse response)
    {
        // Arrange
        sut.Mediator.AsMock()
            .Setup(x => x.Send(It.IsAny<GetDiskByNameQueryRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var act = await sut.GetByName(It.IsAny<string>());

        // Assert
        act.Should().BeOfType<OkObjectResult>();
        sut.Mediator.AsMock()
            .Verify(x => x.Send(It.IsAny<GetDiskByNameQueryRequest>(), It.IsAny<CancellationToken>()),
                Times.Once);
    }
}

public class CreateDiskShould
{
    [Theory]
    [AutoApiData]
    public async Task ReturnsOkResult([NoAutoProperties] DiskController sut, CreateDiskCommand command)
    {
        // Arrange
        sut.Mediator.AsMock()
            .Setup(x => x.Send(It.IsAny<CreateDiskCommandRequest>(), It.IsAny<CancellationToken>()))
            .Verifiable();

        // Act
        var act = await sut.CreateDisk(command);

        // Assert
        act.Should().BeOfType<OkObjectResult>();
        sut.Mediator.AsMock()
            .Verify(x => x.Send(It.IsAny<CreateDiskCommandRequest>(), It.IsAny<CancellationToken>()),
                Times.Once);
    }
}

public class AttachAuthorShould
{
    [Theory]
    [AutoApiData]
    public async Task ReturnsOkResult([NoAutoProperties] DiskController sut, AttachAuthorCommand command)
    {
        // Arrange
        sut.Mediator.AsMock()
            .Setup(x => x.Send(It.IsAny<AttachAuthorCommandRequest>(), default))
            .Verifiable();
        // Act
        var act = await sut.AttachAuthor(command);

        // Assert
        act.Should().BeOfType<OkObjectResult>();
        sut.Mediator.AsMock()
            .Verify(x => x.Send(It.IsAny<AttachAuthorCommandRequest>(), It.IsAny<CancellationToken>()),
                Times.Once);
    }
}

public class DeleteDiskByIdShould
{
    [Theory]
    [AutoApiData]
    public async Task ReturnsNoContentResult([NoAutoProperties] DiskController sut)
    {
        // Arrange
        sut.Mediator.AsMock()
            .Setup(x => x.Send(It.IsAny<DeleteDiskCommandRequest>(), It.IsAny<CancellationToken>()))
            .Verifiable();

        // Act
        var act = await sut.DeleteDiskById(It.IsAny<Guid>());

        // Assert
        act.Should().BeOfType<NoContentResult>();
        sut.Mediator.AsMock()
            .Verify(x => x.Send(It.IsAny<DeleteDiskCommandRequest>(), It.IsAny<CancellationToken>()),
                Times.Once);
    }
}

public class DeleteDiskByNameShould
{
    [Theory]
    [AutoApiData]
    public async Task ReturnsNoContentResult([NoAutoProperties] DiskController sut)
    {
        // Arrange
        sut.Mediator.AsMock()
            .Setup(x => x.Send(It.IsAny<DeleteDiskCommandRequest>(), It.IsAny<CancellationToken>()))
            .Verifiable();

        // Act
        var act = await sut.DeleteDiskByName(It.IsAny<string>());

        // Assert
        act.Should().BeOfType<NoContentResult>();
        sut.Mediator.AsMock()
            .Verify(x => x.Send(It.IsAny<DeleteDiskCommandRequest>(), It.IsAny<CancellationToken>()),
                Times.Once);
    }
}

public class DeleteDiskByPropertyShould
{
    [Theory]
    [AutoApiData]
    public async Task ReturnsNoContentResult([NoAutoProperties] DiskController sut, DeleteDiskCommand command)
    {
        // Arrange
        sut.Mediator.AsMock()
            .Setup(x => x.Send(It.IsAny<DeleteDiskCommandRequest>(), It.IsAny<CancellationToken>()))
            .Verifiable();

        // Act
        var act = await sut.DeleteDiskByProperty(command);

        // Assert
        act.Should().BeOfType<NoContentResult>();
        sut.Mediator.AsMock()
            .Verify(x => x.Send(It.IsAny<DeleteDiskCommandRequest>(), It.IsAny<CancellationToken>()),
                Times.Once);
    }
}

public class UpdateDiskShould
{
    [Theory]
    [AutoApiData]
    public async Task ReturnsOkResult([NoAutoProperties] DiskController sut, UpdateDiskCommand command)
    {
        // Arrange
        sut.Mediator.AsMock()
            .Setup(x => x.Send(It.IsAny<UpdateDiskCommandRequest>(), It.IsAny<CancellationToken>()))
            .Verifiable();

        // Act
        var result = await sut.UpdateDisk(command);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        sut.Mediator.AsMock()
            .Verify(x => x.Send(It.IsAny<UpdateDiskCommandRequest>(), It.IsAny<CancellationToken>()),
                Times.Once);
    }
}
