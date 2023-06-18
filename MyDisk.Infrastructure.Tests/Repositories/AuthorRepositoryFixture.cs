using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MyDisk.Domain.Entities;
using MyDisk.Infrastructure.Repositories;
using MyDisk.Tests.Infrastructure;
using MyDisk.Tests.Utils;

namespace MyDisk.Infrastructure.Tests.Repositories;

public class AuthorRepositoryFixture
{
    public class GetAuthorByFilterAsyncFixture
    {
        // [Theory]
        // [AutoInfrastructureData]
        // public async Task Should_Use_Context
        // (
        //     Expression<Func<Author, bool>> func,
        //     Author author,
        //     DbSet<Author> dbSet,
        //     AuthorRepository sut
        // )
        // {
        //     // Arrange
        //     var authors = new[] { author, author }.AsQueryable();
        //
        //     sut.DbContext.AsMock()
        //         .Setup(_ => _.Authors)
        //         .Returns(dbSet);
        //
        //     var act = sut.GetAuthorByFilterAsync(func);
        //
        //     act.Result.Should().NotBeNull();
        // }
    }
}