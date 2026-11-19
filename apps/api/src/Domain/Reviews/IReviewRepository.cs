using System.Linq.Expressions;

namespace MyDisks.Domain.Reviews;

public interface IReviewRepository
{
    Task<IReadOnlyList<Review>> GetReviewsWithDisksAsync(CancellationToken cancellationToken);
    Task<Review?> GetReviewByFilterAsync(Expression<Func<Review, bool>> predicate);
    Task UpdateReviewAsync(Review review);
}
