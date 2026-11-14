using Polly;

namespace MyDisks.Api.Resiliences;

public static class RetryPolicy
{
    public static IAsyncPolicy<T> Create<T>(Func<T, bool> resultPredicate, int maxRetry,
        int policyTimeRetryMilliSeconds)
    {
        return Policy.HandleResult(resultPredicate)
            .WaitAndRetryAsync(maxRetry, _ => TimeSpan.FromMilliseconds(policyTimeRetryMilliSeconds));
    }
}