using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using MyDisks.Data;
using MyDisks.Domain.Disks;
using Tests.Data;

namespace Data.Tests;

public class DiskRepositoryFixture
{
    public class GetDiskByFilterAsyncFixture
    {
        [Theory]
        [AutoPersistenceData]
        public async Task Should_Get_Disk(Disk disk)
        {
            // Arrange
            var context = new Mock<IApplicationDbContext>();
            var sut = new DiskRepository(context.Object);
            IList<Disk> disks = new List<Disk> { disk };

            context
                .SetupGet(x => x.Disks)
                .ReturnsDbSet(disks);
        
            // Act
            var act = await sut.GetDiskByFilterAsync(x => x.Id == disk.Id);

            // Arrange
            act.Should().NotBeNull();
        }
    }

}