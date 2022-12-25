using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyDisk.Domain.Entities;
using MyDisk.Domain.Interfaces.IRepositories;
using MyDisk.Infrastructure.Interfaces;
using MyDisk.Infrastructure.Persistence;

namespace MyDisk.Infrastructure.Repositories;

public class AuthorRepository : IAuthorRepository
{
    private readonly IApplicationDbContext _context;

    public AuthorRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Author?> GetAuthorByFilterAsync(Expression<Func<Author, bool>> predicate) => await _context.Authors.FirstOrDefaultAsync(predicate);
}