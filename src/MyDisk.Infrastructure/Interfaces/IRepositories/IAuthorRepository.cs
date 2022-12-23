using System.Linq.Expressions;
using MyDisk.Domain.Models;

namespace MyDisk.Infrastructure.Interfaces.IRepositories;

public interface IAuthorRepository
{
    public Task<Author?> GetAuthorByFilterAsync(Expression<Func<Author, bool>> predicate);
}