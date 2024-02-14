using Moq;
using MyDisks.Domain.Disks;
using MyDisks.Services.Disks;
using MyDisks.Tests.Services;
using MyDisks.Tests.Utils;

namespace MyDisks.Services.Tests.Disks;

public class CreateDiskCommandHandlerFixture
{
    [Theory]
    [AutoServiceData]
    public async Task Should_CreateDiskAsync
    (
        Name diskName,
        DateTime releaseDate,
        CreateDiskCommandHandler sut
    )
    {
        // Arrange
        var request = new CreateDiskCommandRequest
        {
            Name = diskName,
            ReleaseDate = releaseDate
        };

        // Act
        await sut.Handle(request, CancellationToken.None);

        // Assert
        sut.DiskRepository.AsMock()
            .Verify(x => x.CreateDiskAsync(It.Is<Disk>(disk =>
                disk.Name == request.Name
                && disk.ReleaseDate == request.ReleaseDate)));
    }
}