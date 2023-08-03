using MyDisks.Contracts.Disks;
using MyDisks.Services.Disks;
using MyDisks.Tests.Api;
using MyDisks.Tests.Utils;

namespace MyDisks.Api.Tests.Controllers;

public class DiskControllerFixture
{
    public class GetAllDisksFixture
    {
        [Theory]
        [AutoApiData]
        public async void Should_Use_Mediator([NoAutoProperties] DiskController sut)
        {
            // Act
            var act = await sut.GetAllDisks();

            // Assert
            sut.Mediator.AsMock()
                .Verify(_ => _.Send(It.IsAny<GetAllDisksQueryRequest>(), CancellationToken.None), Times.Once);
        }

        [Theory]
        [AutoApiData]
        public async void Should_Returns_OkObjectResult([NoAutoProperties] DiskController sut)
        {
            // Act
            var act = await sut.GetAllDisks();

            // Assert
            act.Should().BeOfType<OkObjectResult>();
        }

        [Theory]
        [AutoApiData]
        public async void Should_Returns_Values
        (
            IEnumerable<DiskResult> diskResponses,
            [NoAutoProperties] DiskController sut
        )
        {
            // Arrange
            sut.Mediator.AsMock()
                .Setup(_ => _.Send(It.IsAny<GetAllDisksQueryRequest>(), CancellationToken.None))
                .ReturnsAsync(diskResponses);

            // Act
            var act = await sut.GetAllDisks() as OkObjectResult;
            var result = (IEnumerable<DiskResult>)act!.Value!;

            // Assert
            result.Should().NotBeEmpty();
        }

        [Theory]
        [AutoApiData]
        public async void Should_Returns_Empty_Values([NoAutoProperties] DiskController sut)
        {
            // Arrange
            sut.Mediator.AsMock()
                .Setup(_ => _.Send(It.IsAny<GetAllDisksQueryRequest>(), CancellationToken.None))
                .ReturnsAsync(Array.Empty<DiskResult>());

            // Act
            var act = await sut.GetAllDisks() as OkObjectResult;
            var result = (IEnumerable<DiskResult>)act!.Value!;

            // Assert
            result.Should().BeEmpty();
        }
    }

    public class GetDiskByNameFixture
    {
        [Theory]
        [AutoApiData]
        public async void Should_Use_Mediator
        (
            string diskName,
            [NoAutoProperties] DiskController sut
        )
        {
            // Act
            var act = await sut.GetByName(diskName);

            // Assert
            sut.Mediator.AsMock()
                .Verify(_ => _.Send(It.IsAny<GetDiskByNameQueryRequest>(), CancellationToken.None), Times.Once);
        }

        [Theory]
        [AutoApiData]
        public async void Should_Returns_OkObjectResult
        (
            string diskName,
            [NoAutoProperties] DiskController sut
        )
        {
            // Act
            var act = await sut.GetByName(diskName);

            // Assert
            act.Should().BeOfType<OkObjectResult>();
        }

        [Theory]
        [AutoApiData]
        public async void Should_Returns_Value
        (
            DiskResult diskResponse,
            string diskName,
            [NoAutoProperties] DiskController sut
        )
        {
            // Arrange
            sut.Mediator.AsMock()
                .Setup(_ => _.Send(It.IsAny<GetDiskByNameQueryRequest>(), CancellationToken.None))
                .ReturnsAsync(diskResponse);

            // Act
            var act = await sut.GetByName(diskName) as OkObjectResult;
            var result = (DiskResult)act!.Value!;

            // Assert
            result.Should().NotBeNull();
        }

        [Theory]
        [AutoApiData]
        public async void Should_Map_Request
        (
            string diskName,
            [NoAutoProperties] DiskController sut
        )
        {
            // Act
            var act = await sut.GetByName(diskName);

            // Assert
            sut.Mediator.AsMock()
                .Verify(_ =>
                    _.Send(It.Is<GetDiskByNameQueryRequest>(request => request.Name == diskName),
                        CancellationToken.None), Times.Once);
        }
    }

    public class CreateDiskFixture
    {
        [Theory]
        [AutoApiData]
        public async Task Should_Use_Mediator
        (
            CreateDiskCommand command,
            [NoAutoProperties] DiskController sut
        )
        {
            // Act
            var act = await sut.CreateDisk(command);

            // Assert
            sut.Mediator.AsMock()
                .Verify(_ => _.Send(It.IsAny<CreateDiskCommandRequest>(), CancellationToken.None), Times.Once);
        }

        [Theory]
        [AutoApiData]
        public async Task Should_Returns_NoContentResult
        (
            CreateDiskCommand command,
            [NoAutoProperties] DiskController sut
        )
        {
            // Act
            var act = await sut.CreateDisk(command);

            // Assert
            act.Should().BeOfType<NoContentResult>();
        }

        [Theory]
        [AutoApiData]
        public async Task Should_Map_Request
        (
            CreateDiskCommand command,
            [NoAutoProperties] DiskController sut
        )
        {
            // Act
            var act = await sut.CreateDisk(command);

            // Assert
            sut.Mediator.AsMock()
                .Verify(_ =>
                    _.Send(
                        It.Is<CreateDiskCommandRequest>(request =>
                            request.Name == command.Name && request.ReleaseDate == command.ReleaseDate),
                        CancellationToken.None));
        }
    }

    public class AttachAuthorFixture
    {
        [Theory]
        [AutoApiData]
        public async Task Should_Use_Mediator
        (
            AttachAuthorCommand command,
            [NoAutoProperties] DiskController sut
        )
        {
            // Act
            var act = await sut.AttachAuthor(command);

            // Assert
            sut.Mediator.AsMock()
                .Verify(_ => _.Send(It.IsAny<AttachAuthorCommandRequest>(), CancellationToken.None),
                    Times.Once);
        }

        [Theory]
        [AutoApiData]
        public async Task Should_Returns_NoContentResult
        (
            AttachAuthorCommand command,
            [NoAutoProperties] DiskController sut
        )
        {
            // Act
            var act = await sut.AttachAuthor(command);

            // Assert
            act.Should().BeOfType<NoContentResult>();
        }

        [Theory]
        [AutoApiData]
        public async Task Should_Map_Request
        (
            AttachAuthorCommand command,
            [NoAutoProperties] DiskController sut
        )
        {
            // Act
            var act = await sut.AttachAuthor(command);

            // Assert
            sut.Mediator.AsMock()
                .Verify(_ =>
                    _.Send(
                        It.Is<AttachAuthorCommandRequest>(request =>
                            request.DiskId == command.DiskId && request.AuthorId == command.AuthorId),
                        CancellationToken.None));
        }
    }

    public class DeleteDiskByIdFixture
    {
        [Theory]
        [AutoApiData]
        public async Task Should_Use_Mediator
        (
            Guid diskId,
            [NoAutoProperties] DiskController sut
        )
        {
            // Act
            var act = await sut.DeleteDiskById(diskId);

            // Assert
            sut.Mediator.AsMock()
                .Verify(_ => _.Send(It.IsAny<DeleteDiskCommandRequest>(), CancellationToken.None),
                    Times.Once);
        }

        [Theory]
        [AutoApiData]
        public async Task Should_Returns_NoContentResult
        (
            Guid diskId,
            [NoAutoProperties] DiskController sut
        )
        {
            // Act
            var act = await sut.DeleteDiskById(diskId);

            // Assert
            act.Should().BeOfType<NoContentResult>();
        }

        [Theory]
        [AutoApiData]
        public async Task Should_Map_Request
        (
            Guid diskId,
            [NoAutoProperties] DiskController sut
        )
        {
            // Act
            var act = await sut.DeleteDiskById(diskId);

            // Assert
            sut.Mediator.AsMock()
                .Verify(_ => _.Send(It.Is<DeleteDiskCommandRequest>(request =>
                        request.Value == diskId.ToString() && request.Property == DeleteDiskByProperty.Id),
                    CancellationToken.None));
        }
    }

    public class DeleteDiskByNameFixture
    {
        [Theory]
        [AutoApiData]
        public async Task Should_Use_Mediator
        (
            string diskName,
            [NoAutoProperties] DiskController sut
        )
        {
            // Act
            var act = await sut.DeleteDiskByName(diskName);

            // Assert
            sut.Mediator.AsMock()
                .Verify(x => x.Send(It.IsAny<DeleteDiskCommandRequest>(), CancellationToken.None),
                    Times.Once);
        }

        [Theory]
        [AutoApiData]
        public async Task Should_Returns_NoContentResult
        (
            string diskName,
            [NoAutoProperties] DiskController sut
        )
        {
            // Act
            var act = await sut.DeleteDiskByName(diskName);

            // Assert
            act.Should().BeOfType<NoContentResult>();
        }

        [Theory]
        [AutoApiData]
        public async Task Should_Map_Request
        (
            string diskName,
            [NoAutoProperties] DiskController sut
        )
        {
            // Act
            var act = await sut.DeleteDiskByName(diskName);

            // Assert
            sut.Mediator.AsMock()
                .Verify(_ => _.Send(It.Is<DeleteDiskCommandRequest>(request =>
                        request.Value == diskName && request.Property == DeleteDiskByProperty.Name),
                    CancellationToken.None));
        }
    }

    public class DeleteDiskByPropertyFixture
    {
        [Theory]
        [AutoApiData]
        public async Task Should_Use_Mediator
        (
            DeleteDiskCommand command,
            [NoAutoProperties] DiskController sut
        )
        {
            // Act
            var act = await sut.DeleteDiskByProperty(command);

            // Assert
            sut.Mediator.AsMock()
                .Verify(x => x.Send(It.IsAny<DeleteDiskCommandRequest>(), CancellationToken.None),
                    Times.Once);
        }

        [Theory]
        [AutoApiData]
        public async Task Should_Returns_NoContentResult
        (
            DeleteDiskCommand command,
            [NoAutoProperties] DiskController sut
        )
        {
            // Act
            var act = await sut.DeleteDiskByProperty(command);

            // Assert
            act.Should().BeOfType<NoContentResult>();
        }

        [Theory]
        [AutoApiData]
        public async Task Should_Map_Request
        (
            DeleteDiskCommand command,
            [NoAutoProperties] DiskController sut
        )
        {
            // Act
            var act = await sut.DeleteDiskByProperty(command);

            // Assert
            sut.Mediator.AsMock()
                .Verify(_ => _.Send(It.Is<DeleteDiskCommandRequest>(request =>
                    request.Value == command.Value && request.Property == command.Property), CancellationToken.None));
        }
    }

    public class UpdateDiskFixture
    {
        [Theory]
        [AutoApiData]
        public async Task Should_Use_Mediator
        (
            UpdateDiskCommand command,
            [NoAutoProperties] DiskController sut
        )
        {
            // Act
            var act = await sut.UpdateDisk(command);

            // Assert
            sut.Mediator.AsMock()
                .Verify(x => x.Send(It.IsAny<UpdateDiskCommandRequest>(), CancellationToken.None),
                    Times.Once);
        }

        [Theory]
        [AutoApiData]
        public async Task Should_Returns_OkObjectResult
        (
            UpdateDiskCommand command,
            [NoAutoProperties] DiskController sut
        )
        {
            // Act
            var act = await sut.UpdateDisk(command);

            // Assert
            act.Should().BeOfType<OkObjectResult>();
        }

        [Theory]
        [AutoApiData]
        public async Task Should_Map_Request
        (
            UpdateDiskCommand command,
            [NoAutoProperties] DiskController sut
        )
        {
            // Act
            var act = await sut.UpdateDisk(command);

            // Assert
            sut.Mediator.AsMock()
                .Verify(_ => _.Send(It.Is<UpdateDiskCommandRequest>(request =>
                        request.ReleaseDate == command.ReleaseDate
                        && request.Name == command.Name
                        && request.Id == command.Id),
                    CancellationToken.None));
        }
    }
}
