using MediatR;

namespace MediatorExtension;

public class Request<TRequest, TResponse> : IRequest<TResponse>
{
    public TRequest Value { get; init; } = default!;
}