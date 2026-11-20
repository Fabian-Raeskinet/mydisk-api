using System.Net;
using MyDisks.Frontend.Models;

namespace MyDisks.Frontend.Services;

public sealed class ReviewModerationWatcher(DiskApiClient api)
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(60);
    private static readonly TimeSpan InitialDelay = TimeSpan.FromSeconds(1);
    private static readonly TimeSpan MaxDelay = TimeSpan.FromSeconds(8);

    public async Task<ReviewResult?> WaitForDecisionAsync(Guid reviewId, CancellationToken ct)
    {
        var deadline = DateTimeOffset.UtcNow + Timeout;
        var delay = InitialDelay;

        while (true)
        {
            await Task.Delay(WithJitter(delay), ct);
            delay = TimeSpan.FromTicks(Math.Min(delay.Ticks * 2, MaxDelay.Ticks));

            try
            {
                var review = await api.GetReviewAsync(reviewId, ct);
                if (review.Status != ReviewStatus.Pending)
                    return review;
            }
            catch (ApiException error) when (error.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            catch (Exception error) when (IsTransient(error, ct))
            {
            }

            if (DateTimeOffset.UtcNow >= deadline)
                throw new TimeoutException();
        }
    }

    private static bool IsTransient(Exception error, CancellationToken ct) => error switch
    {
        ApiException api => (int)api.StatusCode >= 500 || api.StatusCode is HttpStatusCode.RequestTimeout or HttpStatusCode.TooManyRequests,
        HttpRequestException => true,
        OperationCanceledException => !ct.IsCancellationRequested,
        _ => false
    };

    private static TimeSpan WithJitter(TimeSpan delay) =>
        delay * (0.8 + Random.Shared.NextDouble() * 0.4);
}
