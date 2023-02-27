namespace MyDisk.RetryService;

public interface IRetryService
{
    Task<TResponse> ExecuteAsync<TException, TResponse>(Func<Task<TResponse>> func)
        where TException : Exception;

    Task<TResponse> ExecuteAsync<TException, TResponse>(Func<Task<TResponse>> func,
        RetryServiceSettings settings) where TException : Exception;

    Task<TResponse> ExecuteAsync<TResponse>(Func<TResponse, bool> resultPredicate, Func<Task<TResponse>> func, RetryServiceSettings settings);
    Task<TResponse> ExecuteAsync<TResponse>(Func<TResponse, bool> resultPredicate, Func<Task<TResponse>> func);
}