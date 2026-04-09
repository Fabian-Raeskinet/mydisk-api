using MediatR;

namespace MyDisks.Services.Behaviors;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public TransactionBehavior(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var response = await next();

            await _unitOfWork.CommitAsync();

            return response;
        }
        catch (Exception)
        {
            
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}