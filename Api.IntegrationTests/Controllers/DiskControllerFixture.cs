using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Moq;
using MyDisks.Contracts.Disks;
using MyDisks.Domain.Authors;
using MyDisks.Domain.Disks;
using MyDisks.IntegrationTests.Api;

namespace MyDisks.Api.IntegrationTests.Controllers;

public sealed class DiskControllerFixture
{
    public sealed class GetAllDisksFixture : ControllerFixtureBase
    {
        [Theory]
        [AutoApiData]
        public async Task Should_HttpStatusCode_Be_Ok(List<Disk> disks)
        {
            // Arrange
            _factory.DiskRepositoryMock
                .Setup(x => x.GetDisksAsync())
                .ReturnsAsync(disks);

            // Act
            var act = await _client.GetAsync("/api/disk/disks");

            // Assert
            act.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }

    public sealed class GetByNameFixture : ControllerFixtureBase
    {
        [Theory]
        [AutoApiData]
        public async Task Should_HttpStatusCode_Be_Ok(string name, Disk? disk)
        {
            // Arrange
            _factory.DiskRepositoryMock
                .Setup(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()))
                .ReturnsAsync(disk);

            // Act
            var act = await _client.GetAsync($"/api/disk/disk-by-name?name={name}");

            // Assert
            act.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Theory]
        [AutoApiData]
        public async Task Should_HttpStatusCode_Be_NotFound(string name)
        {
            // Arrange
            _factory.DiskRepositoryMock
                .Setup(x => x.GetDiskByFilterAsync(d => d.Name == name))
                .ReturnsAsync((Disk)null);

            // Act
            var act = await _client.GetAsync($"/api/disk/disk-by-name?name={name}");

            // Assert
            act.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Should_HttpStatusCode_Be_BadRequest()
        {
            // Act
            var act = await _client.GetAsync("/api/disk/disk-by-name?name=");

            // Assert
            act.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }

    public class CreateDiskCommandFixture : ControllerFixtureBase
    {
        [Theory]
        [AutoApiData]
        public async Task Should_HttpStatusCode_Be_NoContent(Name diskName, DateTime releaseDate, Guid diskId)
        {
            // Arrange
            var command = new CreateDiskCommand
            {
                Name = diskName,
                ReleaseDate = releaseDate
            };

            var content = JsonContent.Create(command);

            _factory.DiskRepositoryMock
                .Setup(x => x.CreateDiskAsync(It.Is<Disk>(disk =>
                    disk.Name == command.Name && disk.ReleaseDate == command.ReleaseDate)))
                .ReturnsAsync(diskId);

            // Act
            var act = await _client.PostAsync("/api/disk/create-disk", content);
            // Assert
            act.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Theory]
        [AutoApiData]
        public async Task Should_HttpStatusCode_Be_BadRequest(DateTime releaseDate)
        {
            // Arrange
            var command = new CreateDiskCommand { Name = null, ReleaseDate = releaseDate };
            var content = JsonContent.Create(command);

            // Act
            var act = await _client.PostAsync("/api/disk/create-disk", content);
            // Assert
            act.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }

    public class AttachAuthorCommandFixture : ControllerFixtureBase
    {
        [Theory]
        [AutoApiData]
        public async Task Should_HttpStatusCode_Be_NoContent(AttachAuthorCommand command, Disk disk, Author author)
        {
            // Arrange
            var content = JsonContent.Create(command);

            _factory.DiskRepositoryMock
                .Setup(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()))
                .ReturnsAsync(disk);

            _factory.AuthorRepositoryMock
                .Setup(x => x.GetAuthorByFilterAsync(It.IsAny<Expression<Func<Author, bool>>>()))
                .ReturnsAsync(author);

            // Act
            var act = await _client.PostAsync($"/api/disk/attach-author", content);

            // Assert
            act.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Should_HttpStatusCode_Be_BadRequest()
        {
            // Arrange
            var command = new AttachAuthorCommand { DiskId = Guid.Empty };
            var content = JsonContent.Create(command);

            _factory.DiskRepositoryMock
                .Setup(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()))
                .ReturnsAsync((Disk)null!);

            // Act
            var act = await _client.PostAsync($"/api/disk/attach-author", content);

            // Assert
            act.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }

    public class DeleteDiskCommandFixture : ControllerFixtureBase
    {
        [Theory]
        [AutoApiData]
        public async Task Should_HttpStatusCode_Be_NoContent(Disk disk)
        {
            // Arrange
            _factory.DiskRepositoryMock
                .Setup(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()))
                .ReturnsAsync(disk);

            // Act
            var act = await _client.DeleteAsync($"/api/disk/delete-disk?diskId={disk.Id}");

            // Assert
            act.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Theory]
        [AutoApiData]
        public async Task Should_HttpStatusCode_Be_NotFound(Guid diskId)
        {
            // Arrange
            _factory.DiskRepositoryMock
                .Setup(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()))
                .ReturnsAsync((Disk)null);

            // Act
            var act = await _client.DeleteAsync($"/api/disk/delete-disk?diskId={diskId}");

            // Assert
            act.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Should_HttpStatusCode_Be_BadRequest()
        {
            // Act
            var act = await _client.DeleteAsync("/api/disk/delete-disk?diskId=");

            // Assert
            act.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }

    public class UpdateDiskCommandFixture : ControllerFixtureBase
    {
        [Theory]
        [AutoApiData]
        public async Task Should_HttpStatusCode_Be_NoContent(Guid diskId, Name diskName, Disk disk)
        {
            // Arrange
            var command = new UpdateDiskCommand
            {
                Id = diskId,
                Name = diskName
            };
            var content = JsonContent.Create(command);
            _factory.DiskRepositoryMock
                .Setup(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()))
                .ReturnsAsync(disk);

            // Act
            var act = await _client.PutAsync($"/api/disk/update-disk", content);

            // Assert
            act.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Should_HttpStatusCode_Be_BadRequest()
        {
            // Arrange
            var command = new UpdateDiskCommand { Id = Guid.Empty };
            var content = JsonContent.Create(command);

            // Act
            var act = await _client.PutAsync($"/api/disk/update-disk", content);

            // Assert
            act.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Theory]
        [AutoApiData]
        public async Task Should_HttpStatusCode_Be_NotFound(UpdateDiskCommand command)
        {
            // Arrange
            var content = JsonContent.Create(command);
            _factory.DiskRepositoryMock
                .Setup(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()))
                .ReturnsAsync((Disk)null!);

            // Act
            var act = await _client.PutAsync($"/api/disk/update-disk", content);

            // Assert
            act.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}