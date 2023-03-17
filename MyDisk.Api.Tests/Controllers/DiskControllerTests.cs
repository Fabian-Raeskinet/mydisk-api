using MyDisk.Services.Disks.DTOs;
using MyDisk.Services.Disks.Requests;
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
            .Setup(x => x.Send(It.IsAny<GetAllDisksRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DiskEntity>());
        // Act
        var act = await sut.GetAllDisks();

        // Assert
        act.Should().BeOfType<OkObjectResult>();
        sut.Mediator.AsMock()
            .Verify(x => x.Send(It.IsAny<GetAllDisksRequest>(), It.IsAny<CancellationToken>()), Times.Once);
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
            .Setup(x => x.Send(It.IsAny<GetDiskByNameRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var act = await sut.GetByName(It.IsAny<string>());

        // Assert
        act.Should().BeOfType<OkObjectResult>();
        sut.Mediator.AsMock()
            .Verify(x => x.Send(It.IsAny<GetDiskByNameRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [AutoApiData]
    public async void ReturnsNotFound([NoAutoProperties] DiskController sut)
    {
        // Arrange
        DiskResponse? response = null;
        sut.Mediator.AsMock()
            .Setup(x => x.Send(It.IsAny<GetDiskByNameRequest>(), It.IsAny<CancellationToken>()))!
            .ReturnsAsync(response);

        // Act
        var act = await sut.GetByName(It.IsAny<string>());

        // Assert
        act.Should().BeOfType<NotFoundResult>();
        sut.Mediator.AsMock()
            .Verify(x => x.Send(It.IsAny<GetDiskByNameRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    // [Theory]
    // [AutoDomainData]
    // public async Task ReturnsBadRequest
    // (
    //     [Frozen] Mock<IMediator> sut.Mediator.AsMock()

    // )
    // {
    //     var request = new EanValidationRequest();
    //     var del = new Mock<RequestHandlerDelegate<Unit>>();
    //     var sut = new ValidatorPipelineBehavior<EanValidationRequest, Unit>(
    //         new List<IValidator<EanValidationRequest>> { new EanValidationRequestValidator() });
    //
    //     //Act
    //     Func<Task> act = async () => await sut.Handle(request, del.Object, default);
    //
    //     await act.Should().ThrowAsync<ValidationException>();
    // }

    public class CreateDiskShould
    {
        [Theory]
        [AutoApiData]
        public async Task ReturnsOkResult([NoAutoProperties] DiskController sut, Guid diskId)
        {
            // Arrange
            sut.Mediator.AsMock()
                .Setup(x => x.Send(It.IsAny<CreateDiskRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(diskId);

            // Act
            var act = await sut.CreateDisk(It.IsAny<CreateDiskRequest>());

            // Assert
            act.Should().BeOfType<OkObjectResult>();
            sut.Mediator.AsMock()
                .Verify(x => x.Send(It.IsAny<CreateDiskRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }

    public class AttachAuthorShould
    {
        [Theory]
        [AutoApiData]
        public async Task ReturnsOkResult([NoAutoProperties] DiskController sut)
        {
            // Act
            var act = await sut.AttachAuthor(It.IsAny<AttachAuthorRequest>());

            // Assert
            act.Should().BeOfType<OkObjectResult>();
            sut.Mediator.AsMock()
                .Verify(x => x.Send(It.IsAny<AttachAuthorRequest>(), It.IsAny<CancellationToken>()), Times.Once);
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
                .Setup(x => x.Send(It.IsAny<DeleteDiskRequest>(), It.IsAny<CancellationToken>()))
                .Verifiable();

            // Act
            var act = await sut.DeleteDiskById(It.IsAny<Guid>());

            // Assert
            act.Should().BeOfType<NoContentResult>();
            sut.Mediator.AsMock()
                .Verify(x => x.Send(It.IsAny<DeleteDiskRequest>(), It.IsAny<CancellationToken>()), Times.Once);
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
                .Setup(x => x.Send(It.IsAny<DeleteDiskRequest>(), It.IsAny<CancellationToken>()))
                .Verifiable();

            // Act
            var act = await sut.DeleteDiskByName(It.IsAny<string>());

            // Assert
            act.Should().BeOfType<NoContentResult>();
            sut.Mediator.AsMock()
                .Verify(x => x.Send(It.IsAny<DeleteDiskRequest>(), It.IsAny<CancellationToken>()), Times.Once);
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
                .Setup(x => x.Send(It.IsAny<UpdateDiskRequest>(), It.IsAny<CancellationToken>()))
                .Verifiable();

            // Act
            var result = await sut.UpdateDisk(It.IsAny<UpdateDiskRequest>());

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            sut.Mediator.AsMock()
                .Verify(x => x.Send(It.IsAny<UpdateDiskRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}