using MediatR;
using MyDisk.Contracts.Disks;
using MyDisk.Services;
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
            .Setup(x => x.Send(It.IsAny<Request<GetAllDisksQuery, List<DiskResponse>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DiskResponse>());
        // Act
        var act = await sut.GetAllDisks();

        // Assert
        act.Should().BeOfType<OkObjectResult>();
        sut.Mediator.AsMock()
            .Verify(
                x => x.Send(It.IsAny<Request<GetAllDisksQuery, List<DiskResponse>>>(), It.IsAny<CancellationToken>()),
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
            .Setup(x => x.Send(It.IsAny<Request<GetDiskByNameQuery, DiskResponse>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var act = await sut.GetByName(It.IsAny<string>());

        // Assert
        act.Should().BeOfType<OkObjectResult>();
        sut.Mediator.AsMock()
            .Verify(x => x.Send(It.IsAny<Request<GetDiskByNameQuery, DiskResponse>>(), It.IsAny<CancellationToken>()),
                Times.Once);
    }
}

public class CreateDiskShould
{
    [Theory]
    [AutoApiData]
    public async Task ReturnsOkResult([NoAutoProperties] DiskController sut, Guid diskId)
    {
        // Arrange
        sut.Mediator.AsMock()
            .Setup(x => x.Send(It.IsAny<Request<CreateDiskCommand, Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(diskId);

        // Act
        var act = await sut.CreateDisk(It.IsAny<CreateDiskCommand>());

        // Assert
        act.Should().BeOfType<OkObjectResult>();
        sut.Mediator.AsMock()
            .Verify(x => x.Send(It.IsAny<Request<CreateDiskCommand, Guid>>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class AttachAuthorShould
{
    [Theory]
    [AutoApiData]
    public async Task ReturnsOkResult([NoAutoProperties] DiskController sut)
    {
        // Act
        var act = await sut.AttachAuthor(It.IsAny<AttachAuthorCommand>());

        // Assert
        act.Should().BeOfType<OkObjectResult>();
        sut.Mediator.AsMock()
            .Verify(x => x.Send(It.IsAny<Request<AttachAuthorCommand, Guid>>(), It.IsAny<CancellationToken>()), Times.Once);
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
            .Setup(x => x.Send(It.IsAny<Request<DeleteDiskCommand, Unit>>(), It.IsAny<CancellationToken>()))
            .Verifiable();

        // Act
        var act = await sut.DeleteDiskById(It.IsAny<Guid>());

        // Assert
        act.Should().BeOfType<NoContentResult>();
        sut.Mediator.AsMock()
            .Verify(x => x.Send(It.IsAny<Request<DeleteDiskCommand, Unit>>(), It.IsAny<CancellationToken>()), Times.Once);
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
            .Setup(x => x.Send(It.IsAny<Request<DeleteDiskCommand, Unit>>(), It.IsAny<CancellationToken>()))
            .Verifiable();

        // Act
        var act = await sut.DeleteDiskByName(It.IsAny<string>());

        // Assert
        act.Should().BeOfType<NoContentResult>();
        sut.Mediator.AsMock()
            .Verify(x => x.Send(It.IsAny<Request<DeleteDiskCommand, Unit>>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class DeleteDiskByPropertyShould
{
    [Theory]
    [AutoApiData]
    public async Task ReturnsNoContentResult([NoAutoProperties] DiskController sut, DeleteDiskCommand request)
    {
        // Arrange
        sut.Mediator.AsMock()
            .Setup(x => x.Send(It.IsAny<Request<DeleteDiskCommand, Unit>>(), It.IsAny<CancellationToken>()))
            .Verifiable();

        // Act
        var act = await sut.DeleteDiskByProperty(request);

        // Assert
        act.Should().BeOfType<NoContentResult>();
        sut.Mediator.AsMock()
            .Verify(x => x.Send(It.IsAny<Request<DeleteDiskCommand, Unit>>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class UpdateDiskShould
{
    [Theory]
    [AutoApiData]
    public async Task ReturnsOkResult([NoAutoProperties] DiskController sut)
    {
        // Arrange
        sut.Mediator.AsMock()
            .Setup(x => x.Send(It.IsAny<Request<UpdateDiskCommand, DiskResponse>>(), It.IsAny<CancellationToken>()))
            .Verifiable();

        // Act
        var result = await sut.UpdateDisk(It.IsAny<UpdateDiskCommand>());

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        sut.Mediator.AsMock()
            .Verify(x => x.Send(It.IsAny<Request<UpdateDiskCommand, DiskResponse>>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
