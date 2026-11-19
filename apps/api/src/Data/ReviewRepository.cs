using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyDisks.Domain.Reviews;

namespace MyDisks.Data;

public class ReviewRepository : IReviewRepository
{
    public IApplicationDbContext Context { get; }

    public ReviewRepository(IApplicationDbContext context)
    {
        Context = context;
    }

    public async Task<IReadOnlyList<Review>> GetReviewsWithDisksAsync(CancellationToken cancellationToken)
    {
        return await Context.Reviews
            .AsNoTracking()
            .Include(review => review.Disk)
            .OrderByDescending(review => review.PublishedDate)
            .ThenBy(review => review.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<Review?> GetReviewByFilterAsync(Expression<Func<Review, bool>> predicate)
    {
        return await Context.Reviews
            .FirstOrDefaultAsync(predicate);
    }

    public Task UpdateReviewAsync(Review review)
    {
        Context.Reviews.Update(review);
        return Context.SaveChangesAsync();
    }
}
