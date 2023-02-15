using System.Linq.Expressions;
using MyDisk.Domain.Entities;

namespace MyDisk.Domain.Interfaces.IRepositories;

public interface IAuthorRepository
{
    public Task<Author?> GetAuthorByFilterAsync(Expression<Func<Author, bool>> predicate);
}