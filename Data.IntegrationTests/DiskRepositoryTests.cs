using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using MyDisks.Domain.Disks;

namespace MyDisks.Data.IntegrationTests;

public class DiskRepositoryTests
{
    [Fact]
    public async Task Should()
    {
        var context = new Mock<IApplicationDbContext>();
        var disk = new Disk
        {
            Id = Guid.NewGuid(),
            ReleaseDate = DateTime.Now
        };
        
        IList<Disk> disks = new List<Disk> { disk };

        context
            .SetupGet(x => x.Disks)
            .ReturnsDbSet(disks);
        
        var sut = new DiskRepository(context.Object);

        var act = await sut.GetDiskByFilterAsync(x => x.Id == disk.Id);

        act.Should().NotBeNull();

    }
}