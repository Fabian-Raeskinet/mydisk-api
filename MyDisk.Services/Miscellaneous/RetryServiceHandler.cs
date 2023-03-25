using MediatR;
using MyDisk.RetryService;

namespace MyDisk.Services.Miscellaneous;

public class RetryServiceHandler : IRequestHandler<RetryServiceRequest, Unit>
{
    public RetryServiceHandler(IRetryService retryService)
    {
        RetryService = retryService;
    }

    public IRetryService RetryService { get; }

    public async Task<Unit> Handle(RetryServiceRequest request, CancellationToken cancellationToken)
    {
        await RetryService.ExecuteAsync(r => r == 1,
            () =>
            {
                var number = new Random().Next(1, 3);
                return Task.FromResult(number);
            });


        await RetryService.ExecuteAsync<InvalidOperationException, int>(
            () =>
            {
                var number = new Random().Next(1, 3);
                if (number == 1)
                    throw new InvalidOperationException();
                return Task.FromResult(number);
            });


        return Unit.Value;
    }
}