using MediatR;

namespace MyDisk.Services;

public class Request<TRequest, TResponse> : IRequest<TResponse>
{
    public TRequest Value { get; init; } = default!;
}

public abstract class RequestHandler<TRequest, TResponse> : IRequestHandler<Request<TRequest, TResponse>, TResponse>
{
    public abstract Task<TResponse> Handle(Request<TRequest, TResponse> request, CancellationToken cancellationToken);
}