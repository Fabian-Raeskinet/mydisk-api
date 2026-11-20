using MediatR;

namespace MyDisks.Services;

public interface ICommand<out TResult> : IRequest<TResult>
{
}
