using FluentAssertions;
using MyDisks.Domain.Entities;
using MyDisks.Tests.Domain;
using MyDisks.Data;

namespace MyDisks.Data.IntegrationTests;

public class DiskRepositoryFixture : DatabaseFixtureBase
{
    protected DiskRepositoryFixture()
    {
        Sut = new DiskRepository(DbContext);
    }

    private DiskRepository Sut { get; }

    public class GetDisksAsyncFixture : DiskRepositoryFixture
    {
        [Theory]
        [AutoDomainData]
        public async Task Should_Get_All(Disk disk1, Disk disk2)
        {
            // Arrange
            await Sut.CreateDiskAsync(disk1);
            await Sut.CreateDiskAsync(disk2);
            // Act
            var act = await Sut.GetDisksAsync();

            // Assert
            act.Should().NotBeEmpty();
        }
    }
    
    public class CreateDiskAsyncFixture : DiskRepositoryFixture
    {
        [Theory]
        [AutoDomainData]
        public async Task Should_Persist_New_Disk(Disk disk)
        {
            // Act
            await Sut.CreateDiskAsync(disk);

            // Assert
            var expected = await Sut.GetDiskByFilterAsync(x => x.Id == disk.Id);
            expected.Should()
                .Be(disk);
        }
    }
    
    public class UpdateDiskAsyncFixture : DiskRepositoryFixture
    {
        [Theory]
        [AutoDomainData]
        public async Task Should_Persist_Updated_Disk(Disk disk, string diskName)
        {
            // Arrange
            await Sut.CreateDiskAsync(disk);
            disk.Name = diskName;
            
            // Act
            await Sut.UpdateDiskAsync(disk);
            
            // Assert
            var expected = await Sut.GetDiskByFilterAsync(x => x.Id == disk.Id);
            expected.Should()
                .Be(disk);
        }
    }
    
    public class DeleteDiskAsyncFixture : DiskRepositoryFixture
    {
        [Theory]
        [AutoDomainData]
        public async Task Should_Delete_Disk(Disk disk, string diskName)
        {
            // Arrange
            await Sut.CreateDiskAsync(disk);
            disk.Name = diskName;
            
            // Act
            await Sut.DeleteDiskAsync(disk);
            
            // Assert
            var expected = await Sut.GetDiskByFilterAsync(x => x.Id == disk.Id);
            expected.Should().BeNull();
        }
    }
}