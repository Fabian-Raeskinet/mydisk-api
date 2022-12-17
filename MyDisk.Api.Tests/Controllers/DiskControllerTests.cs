using MediatR;
using MyDisk.Services.Disks.DTOs;
using MyDisk.Services.Disks.Requests;

namespace MyDisk.Api.Tests.Controllers;

public class GetAllDisksShould
{
    [Theory, AutoDomainData]
    public async void ReturnsOkResultTest([Frozen] Mock<IMediator> mediator, [NoAutoProperties] DiskController sut)
    {
        mediator.Setup(x => x.Send(It.IsAny<GetAllDisksRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DiskEntity>());
        var result = await sut.GetAllDisks();
        result.Should().BeOfType<OkObjectResult>();
        mediator.Verify(x => x.Send(It.IsAny<GetAllDisksRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class GetDiskByNameShould
{
    [Theory, AutoDomainData]
    public async void ReturnsOkResult([Frozen] Mock<IMediator> mediator, [NoAutoProperties] DiskController sut,
        DiskResponse response)
    {
        mediator.Setup(x => x.Send(It.IsAny<GetDiskByNameRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);
        var result = await sut.GetByName(It.IsAny<string>());
        result.Should().BeOfType<OkObjectResult>();
        mediator.Verify(x => x.Send(It.IsAny<GetDiskByNameRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory, AutoDomainData]
    public async void ReturnsNotFound([Frozen] Mock<IMediator> mediator, [NoAutoProperties] DiskController sut)
    {
        DiskResponse? response = null;
        mediator.Setup(x => x.Send(It.IsAny<GetDiskByNameRequest>(), It.IsAny<CancellationToken>()))!
            .ReturnsAsync(response);
        var result = await sut.GetByName(It.IsAny<string>());
        result.Should().BeOfType<NotFoundResult>();
        mediator.Verify(x => x.Send(It.IsAny<GetDiskByNameRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    // [Theory, AutoDomainData]
    // public async void ReturnsBadRequest([Frozen] Mock<IMediator> mediator, [NoAutoProperties] DiskController sut)
    // {
    //     string? name = null;
    //     var result = await sut.GetByName(name);
    //     result.Should().BeOfType<BadRequestObjectResult>();
    // }
}

public class CreateDiskShould
{
    [Theory, AutoDomainData]
    public async Task ReturnsOkResult([Frozen] Mock<IMediator> mediator, [NoAutoProperties] DiskController sut, Guid diskId)
    {
        mediator.Setup(x => x.Send(It.IsAny<CreateDiskRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(diskId);
        var result = await sut.CreateDisk(It.IsAny<CreateDiskRequest>());
        result.Should().BeOfType<OkObjectResult>();
        mediator.Verify(x => x.Send(It.IsAny<CreateDiskRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    // [Theory, AutoDomainData]
    // public async Task ReturnsBadRequest([NoAutoProperties] DiskController sut, [NoAutoProperties] CreateDiskRequest request)
    // {
    //     var result = await sut.CreateDisk(request);
    //     result.Should().BeOfType<BadRequestObjectResult>();
    // }
}

public class AttachAuthorShould
{
    [Theory, AutoDomainData]
    public async Task ReturnsOkResult([Frozen] Mock<IMediator> mediator, [NoAutoProperties] DiskController sut)
    {
        var result = await sut.AttachAuthor(It.IsAny<AttachAuthorRequest>());
        result.Should().BeOfType<OkObjectResult>();
        mediator.Verify(x => x.Send(It.IsAny<AttachAuthorRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory, AutoDomainData]
    public async Task ReturnsNotFoundResult([Frozen] Mock<IMediator> mediator, [NoAutoProperties] DiskController sut) =>
        throw new NotImplementedException();

    [Theory, AutoDomainData]
    public async Task ReturnsBadRequest([NoAutoProperties] DiskController sut, [NoAutoProperties] AttachAuthorRequest request)
    {
        var result = await sut.AttachAuthor(request);
        result.Should().BeOfType<BadRequestObjectResult>();
    }
}