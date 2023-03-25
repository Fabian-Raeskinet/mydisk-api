using MediatR;

namespace MediatorExtension;


public abstract class RequestHandler<TRequest, TResponse> : IRequestHandler<Request<TRequest, TResponse>, TResponse>
{
    public abstract Task<TResponse> Handle(Request<TRequest, TResponse> request, CancellationToken cancellationToken);
}