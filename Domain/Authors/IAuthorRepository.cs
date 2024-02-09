using System.Linq.Expressions;

namespace MyDisks.Domain.Authors;

public interface IAuthorRepository
{
    /// <summary>
    /// Retrieves an instance of <see cref="Author"/> that matches the specified filter predicate asynchronously.
    /// </summary>
    /// <param name="predicate">The filter predicate used to match the <see cref="Author"/>.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation. The task result contains an instance
    /// of <see cref="Author"/> if a match is found; otherwise, <c>null</c>.
    /// </returns>
    public Task<Author?> GetAuthorByFilterAsync(Expression<Func<Author, bool>> predicate);
}