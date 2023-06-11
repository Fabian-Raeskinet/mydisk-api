using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyDisk.Domain;
using MyDisk.Domain.Entities;
using MyDisk.Infrastructure.Interfaces;

namespace MyDisk.Infrastructure.Repositories;

public class AuthorRepository : IAuthorRepository
{
    public IApplicationDbContext DbContext { get; }

    public AuthorRepository(IApplicationDbContext context)
    {
        DbContext = context;
    }

    public async Task<Author?> GetAuthorByFilterAsync(Expression<Func<Author, bool>> predicate)
    {
        return await DbContext.Authors.FirstOrDefaultAsync(predicate);
    }
}