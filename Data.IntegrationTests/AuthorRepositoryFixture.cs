using FluentAssertions;
using MyDisks.Domain.Entities;
using MyDisks.Tests.Domain;

namespace MyDisks.Data.IntegrationTests;

public class AuthorRepositoryFixture : DatabaseFixtureBase
{
    public AuthorRepositoryFixture()
    {
        Sut = new AuthorRepository(DbContext);
    }

    private AuthorRepository Sut { get; }

    public class GetDisksAsyncFixture : AuthorRepositoryFixture
    {
        [Theory]
        [AutoDomainData]
        public async Task Should_Get_By_Filter(Disk disk)
        {
            // Arrange
            var diskRepository = new DiskRepository(DbContext);
            await diskRepository.CreateDiskAsync(disk);

            // Act
            var act = await Sut.GetAuthorByFilterAsync(x => x.Id == disk.AuthorId);

            // Assert
            act.Should().Be(disk.Author);
        }
    }
}