using System.Linq.Expressions;
using MyDisks.Domain.Authors;

namespace MyDisks.Domain;

public interface IAuthorRepository
{
    public Task<Author?> GetAuthorByFilterAsync(Expression<Func<Author, bool>> predicate);
}