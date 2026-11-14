using MediatR;

namespace MyDisks.Services;

public interface IQuery<out TResponse> : IRequest<TResponse>
{
    
}