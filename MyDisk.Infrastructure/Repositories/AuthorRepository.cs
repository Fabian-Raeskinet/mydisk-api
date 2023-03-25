using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyDisk.Domain;
using MyDisk.Domain.Entities;
using MyDisk.Infrastructure.Interfaces;

namespace MyDisk.Infrastructure.Repositories;

public class AuthorRepository : IAuthorRepository
{
    private readonly IApplicationDbContext _context;

    public AuthorRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Author?> GetAuthorByFilterAsync(Expression<Func<Author, bool>> predicate)
    {
        return await _context.Authors.FirstOrDefaultAsync(predicate);
    }
}