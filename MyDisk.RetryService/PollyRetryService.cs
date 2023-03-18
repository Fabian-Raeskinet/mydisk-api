using Microsoft.Extensions.Options;
using Polly;

namespace MyDisk.RetryService;

public class PollyRetryService : IRetryService
{
    public PollyRetryService(IOptions<RetryServiceSettings> settings)
    {
        Settings = settings;
    }

    public IOptions<RetryServiceSettings> Settings { get; }

    public async Task<TResponse> ExecuteAsync<TException, TResponse>(Func<Task<TResponse>> func,
        RetryServiceSettings settings) where TException : Exception
    {
        var policy = Policy<TResponse>.Handle<TException>()
            .WaitAndRetryAsync(settings.MaxRetry,
                _ => TimeSpan.FromMilliseconds(settings.PolicyTimeRetryMilliSeconds));

        return await policy.ExecuteAsync(func);
    }

    public async Task<TResponse> ExecuteAsync<TException, TResponse>(Func<Task<TResponse>> func)
        where TException : Exception => await ExecuteAsync<TException, TResponse>(func, Settings.Value);

    public async Task<TResponse> ExecuteAsync<TResponse>(Func<TResponse, bool> resultPredicate, Func<Task<TResponse>> func, RetryServiceSettings settings)
    {
        var policy = Policy.HandleResult(resultPredicate)
            .WaitAndRetryAsync(settings.MaxRetry,
                _ => TimeSpan.FromMilliseconds(settings.PolicyTimeRetryMilliSeconds));

        return await policy.ExecuteAsync(func);
    }
    
    public async Task<TResponse> ExecuteAsync<TResponse>(Func<TResponse, bool> resultPredicate, Func<Task<TResponse>> func)
        => await ExecuteAsync(resultPredicate, func, Settings.Value);
}