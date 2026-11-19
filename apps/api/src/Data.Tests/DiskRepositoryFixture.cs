using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using MyDisks.Domain.Disks;
using MyDisks.Domain.Reviews;
using MyDisks.Tests.Data;
using MyDisks.Tests.Utils;

namespace MyDisks.Data.Tests;

public class DiskRepositoryFixture
{
    public class GetDisksAsyncFixture
    {
        [Theory]
        [AutoPersistenceData]
        public async Task Should_Contains_Disk(Disk disk)
        {
            // Arrange
            var context = new Mock<IApplicationDbContext>();
            var sut = new DiskRepository(context.Object);
            IList<Disk> disks = new List<Disk> { disk };

            context
                .Setup(x => x.Disks)
                .ReturnsDbSet(disks);

            // Act
            var act = await sut.GetDisksAsync(_ => true);

            // Assert
            act.Should().Contain(disk);
        }

        [Theory]
        [AutoPersistenceData]
        public async Task Should_Count_Be_2(Disk disk)
        {
            // Arrange
            var context = new Mock<IApplicationDbContext>();
            var sut = new DiskRepository(context.Object);
            IList<Disk> disks = new List<Disk> { disk, disk };

            context
                .Setup(x => x.Disks)
                .ReturnsDbSet(disks);

            // Act
            var act = await sut.GetDisksAsync(_ => true);

            // Assert
            act.Should().HaveCount(2);
        }
    }

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

            // Assert
            act.Should().NotBeNull();
        }
    }

    public class CreateDiskAsyncFixture
    {
        [Theory]
        [AutoPersistenceData]
        public async Task Should_Create_Disk(Disk disk)
        {
            // Arrange
            var context = new Mock<IApplicationDbContext>();
            var diskDbSet = new Mock<DbSet<Disk>>();

            context
                .Setup(x => x.Disks)
                .Returns(diskDbSet.Object);

            var sut = new DiskRepository(context.Object);

            // Act
            await sut.CreateDiskAsync(disk);

            // Assert
            diskDbSet
                .Verify(x => x.AddAsync(disk, CancellationToken.None));
        }
    }

    public class DeleteDiskAsyncFixture
    {
        [Theory]
        [AutoPersistenceData]
        public async Task Should_Delete_Disk(Disk disk, DiskRepository sut)
        {
            // Arrange
            var dbSetMock = new Mock<DbSet<Disk>>();
            sut.Context.AsMock()
                .Setup(x => x.Disks)
                .Returns(dbSetMock.Object);
            
            await sut.CreateDiskAsync(disk);
            
            // Act
            await sut.DeleteDiskAsync(disk);
            
            // Assert
            dbSetMock
                .Verify(x => x.Remove(disk));
        }
    }
    public class UpdateDiskAsyncFixture
    {
        [Fact]
        public async Task Updates_detached_disk()
        {
            await using var context = CreateContext();
            var disk = new Disk { Id = Guid.NewGuid(), Name = new Name("A record") };
            var repository = new DiskRepository(context);

            await repository.UpdateDiskAsync(disk);

            context.Entry(disk).State.Should().Be(EntityState.Modified);
        }

        [Fact]
        public async Task Changing_tracked_disk_does_not_overwrite_concurrently_archived_review()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            await using var ownerContext = new ApplicationDbContext(options);
            var disk = new Disk { Id = Guid.NewGuid(), Name = new Name("A record") };
            var review = new Review { Id = Guid.NewGuid(), Title = "A review", Content = "Content" };
            disk.AddReview(review);
            ownerContext.Add(disk);
            await ownerContext.SaveChangesAsync();

            await using var moderatorContext = new ApplicationDbContext(options);
            var moderatorReview = await moderatorContext.Reviews.SingleAsync();
            moderatorReview.Archive();
            await moderatorContext.SaveChangesAsync();

            disk.Name = new Name("Updated record");
            disk.AddReview(new Review { Title = "Another review", Content = "Content" });
            await new DiskRepository(ownerContext).UpdateDiskAsync(disk);
            await ownerContext.SaveChangesAsync();

            moderatorContext.ChangeTracker.Clear();
            var storedDisk = await moderatorContext.Disks.Include(item => item.Reviews).SingleAsync();
            storedDisk.Name.Value.Should().Be("Updated record");
            storedDisk.Reviews.Should().Contain(item => item.Id == review.Id && item.Status == ReviewStatus.Archived);
            storedDisk.Reviews.Should().Contain(item => item.Title == "Another review");
        }

        private static ApplicationDbContext CreateContext() => new(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
    }
}
