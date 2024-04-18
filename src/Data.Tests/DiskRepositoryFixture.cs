using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using MyDisks.Domain.Disks;
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
            var act = await sut.GetDisksAsync();

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
            var act = await sut.GetDisksAsync();

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

        [Theory]
        [AutoPersistenceData]
        public async Task Should_Save_Changes(Disk disk)
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
            context
                .Verify(x => x.SaveChangesAsync());
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

        [Theory]
        [AutoPersistenceData]
        public async Task Should_SaveChanges(Disk disk, DiskRepository sut)
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
           sut.Context.AsMock()
               .Verify(x => x.SaveChangesAsync());
        }
    }
    public class UpdateDiskAsyncFixture
    {
        [Theory]
        [AutoPersistenceData]
        public async Task Should_Update_Disk(Disk disk, DiskRepository sut)
        {
            // Arrange
            var dbSetMock = new Mock<DbSet<Disk>>();
            sut.Context.AsMock()
                .Setup(x => x.Disks)
                .Returns(dbSetMock.Object);
            
            await sut.CreateDiskAsync(disk);
            
            // Act
            await sut.UpdateDiskAsync(disk);
            
            // Assert
            dbSetMock
                .Verify(x => x.Update(disk));
        }

        [Theory]
        [AutoPersistenceData]
        public async Task Should_SaveChanges(Disk disk, DiskRepository sut)
        {
            // Arrange
            var dbSetMock = new Mock<DbSet<Disk>>();
            sut.Context.AsMock()
                .Setup(x => x.Disks)
                .Returns(dbSetMock.Object);
            
            await sut.CreateDiskAsync(disk);
            
            // Act
            await sut.UpdateDiskAsync(disk);
            
            // Assert
           sut.Context.AsMock()
               .Verify(x => x.SaveChangesAsync());
        }
    }
}