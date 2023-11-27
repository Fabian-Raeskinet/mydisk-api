using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyDisks.Domain;
using MyDisks.Domain.Entities;

namespace MyDisks.Data;

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