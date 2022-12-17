using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyDisk.Api.Controllers;
using MyDisk.Domain.Tests;
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