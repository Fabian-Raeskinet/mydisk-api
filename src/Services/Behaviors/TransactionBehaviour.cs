using MediatR;

namespace MyDisks.Services.Behaviors;

public class TransactionBehavior<TRequest, TResponse>(IUnitOfWork unitOfWork) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync();

        try
        {
            var response = await next();

            await unitOfWork.CommitAsync();

            return response;
        }
        catch (Exception)
        {
            
            await unitOfWork.RollbackAsync();
            throw;
        }
    }
}