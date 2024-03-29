using MediatR;
using MyDisks.RandomServices;
using MyDisks.RetryService;

namespace MyDisks.Services.Miscellaneous;

public class RetryServiceHandler : ICommandHandler<RetryServiceRequest>
{
    public RetryServiceHandler(IRetryService retryService, IRandomService randomService)
    {
        RetryService = retryService;
        RandomService = randomService;
    }

    public IRetryService RetryService { get; }
    public IRandomService RandomService { get; }

    public async Task Handle(RetryServiceRequest request, CancellationToken cancellationToken)
    {
        await RetryService.ExecuteAsync(r => r == 1, RetryWithPredicateFunc);
        await RetryService.ExecuteAsync<InvalidOperationException, int>(RetryWithInvalidOperationFunc);
    }

    public Task<int> RetryWithPredicateFunc()
    {
        var number = RandomService.GetRandomValue(1, 3);
        return Task.FromResult(number);
    }

    public Task<int> RetryWithInvalidOperationFunc()
    {
        var number = RandomService.GetRandomValue(1, 3);
        if (number == 1)
            throw new InvalidOperationException();
        return Task.FromResult(number);
    }
}