using MyDisk.Contracts.Disks;
using MyDisk.Services.Disks;
using MyDisk.Tests.Api;
using MyDisk.Tests.Utils;

namespace MyDisk.Api.Tests.Controllers;

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
                .Verify(x => x.Send(It.IsAny<GetAllDisksQueryRequest>(), default), Times.Once);
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
    }

    public class GetDiskByNameFixture
    {
        [Theory]
        [AutoApiData]
        public async void Should_Use_Mediator([NoAutoProperties] DiskController sut)
        {
            // Act
            var act = await sut.GetByName(It.IsAny<string>());

            // Assert
            sut.Mediator.AsMock()
                .Verify(x => x.Send(It.IsAny<GetDiskByNameQueryRequest>(), default), Times.Once);
        }

        [Theory]
        [AutoApiData]
        public async void Should_Returns_OkObjectResult([NoAutoProperties] DiskController sut)
        {
            // Act
            var act = await sut.GetByName(It.IsAny<string>());

            // Assert
            act.Should().BeOfType<OkObjectResult>();
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
                .Verify(x => x.Send(It.IsAny<CreateDiskCommandRequest>(), default), Times.Once);
        }

        [Theory]
        [AutoApiData]
        public async Task Should_Returns_OkObjectResult
        (
            CreateDiskCommand command,
            [NoAutoProperties] DiskController sut
        )
        {
            // Act
            var act = await sut.CreateDisk(command);

            // Assert
            act.Should().BeOfType<OkObjectResult>();
        }
    }

    public class AttachAuthorFixture
    {
        [Theory]
        [AutoApiData]
        public async Task Should_Use_Mediator([NoAutoProperties] DiskController sut, AttachAuthorCommand command)
        {
            // Act
            var act = await sut.AttachAuthor(command);

            // Assert
            sut.Mediator.AsMock()
                .Verify(x => x.Send(It.IsAny<AttachAuthorCommandRequest>(), default),
                    Times.Once);
        }

        [Theory]
        [AutoApiData]
        public async Task Should_Returns_OkObjectResult([NoAutoProperties] DiskController sut,
            AttachAuthorCommand command)
        {
            // Act
            var act = await sut.AttachAuthor(command);

            // Assert
            act.Should().BeOfType<OkObjectResult>();
        }
    }

    public class DeleteDiskByIdFixture
    {
        [Theory]
        [AutoApiData]
        public async Task Should_Use_Mediator([NoAutoProperties] DiskController sut, Guid diskId)
        {
            // Act
            var act = await sut.DeleteDiskById(diskId);

            // Assert
            sut.Mediator.AsMock()
                .Verify(x => x.Send(It.IsAny<DeleteDiskCommandRequest>(), default),
                    Times.Once);
        }

        [Theory]
        [AutoApiData]
        public async Task Should_Returns_NoContentResult([NoAutoProperties] DiskController sut, Guid diskId)
        {
            // Act
            var act = await sut.DeleteDiskById(diskId);

            // Assert
            act.Should().BeOfType<NoContentResult>();
        }
    }

    public class DeleteDiskByNameFixture
    {
        [Theory]
        [AutoApiData]
        public async Task Should_Use_Mediator([NoAutoProperties] DiskController sut, string diskName)
        {
            // Act
            var act = await sut.DeleteDiskByName(diskName);

            // Assert
            sut.Mediator.AsMock()
                .Verify(x => x.Send(It.IsAny<DeleteDiskCommandRequest>(), default),
                    Times.Once);
        }

        [Theory]
        [AutoApiData]
        public async Task Should_Returns_NoContentResult([NoAutoProperties] DiskController sut, string diskName)
        {
            // Act
            var act = await sut.DeleteDiskByName(diskName);

            // Assert
            act.Should().BeOfType<NoContentResult>();
        }
    }

    public class DeleteDiskByPropertyFixture
    {
        [Theory]
        [AutoApiData]
        public async Task Should_Use_Mediator([NoAutoProperties] DiskController sut, DeleteDiskCommand command)
        {
            // Act
            var act = await sut.DeleteDiskByProperty(command);

            // Assert
            sut.Mediator.AsMock()
                .Verify(x => x.Send(It.IsAny<DeleteDiskCommandRequest>(), default),
                    Times.Once);
        }

        [Theory]
        [AutoApiData]
        public async Task Should_Returns_NoContentResult([NoAutoProperties] DiskController sut,
            DeleteDiskCommand command)
        {
            // Act
            var act = await sut.DeleteDiskByProperty(command);

            // Assert
            act.Should().BeOfType<NoContentResult>();
        }
    }

    public class UpdateDiskFixture
    {
        [Theory]
        [AutoApiData]
        public async Task Should_Use_Mediator([NoAutoProperties] DiskController sut, UpdateDiskCommand command)
        {
            // Act
            var act = await sut.UpdateDisk(command);

            // Assert
            sut.Mediator.AsMock()
                .Verify(x => x.Send(It.IsAny<UpdateDiskCommandRequest>(), default),
                    Times.Once);
        }

        [Theory]
        [AutoApiData]
        public async Task Should_Returns_OkObjectResult([NoAutoProperties] DiskController sut,
            UpdateDiskCommand command)
        {
            // Act
            var act = await sut.UpdateDisk(command);

            // Assert
            act.Should().BeOfType<OkObjectResult>();
        }
    }
}
