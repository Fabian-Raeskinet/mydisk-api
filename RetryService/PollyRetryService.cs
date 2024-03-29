﻿using Microsoft.Extensions.Options;
using Polly;

namespace MyDisks.RetryService;

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
        var policy = Policy<TResponse>
            .Handle<TException>()
            .WaitAndRetryAsync(settings.RetryCount,
                _ => TimeSpan.FromMilliseconds(settings.TimeRetryMilliSeconds));

        return await policy.ExecuteAsync(func);
    }

    public async Task<TResponse> ExecuteAsync<TException, TResponse>(Func<Task<TResponse>> func)
        where TException : Exception
    {
        return await ExecuteAsync<TException, TResponse>(func, Settings.Value);
    }

    public async Task<TResponse> ExecuteAsync<TResponse>(Func<TResponse, bool> resultPredicate,
        Func<Task<TResponse>> func, RetryServiceSettings settings)
    {
        var policy = Policy
            .HandleResult(resultPredicate)
            .WaitAndRetryAsync(settings.RetryCount,
                _ => TimeSpan.FromMilliseconds(settings.TimeRetryMilliSeconds));

        return await policy.ExecuteAsync(func);
    }

    public async Task<TResponse> ExecuteAsync<TResponse>(Func<TResponse, bool> resultPredicate,
        Func<Task<TResponse>> func)
    {
        return await ExecuteAsync(resultPredicate, func, Settings.Value);
    }
}