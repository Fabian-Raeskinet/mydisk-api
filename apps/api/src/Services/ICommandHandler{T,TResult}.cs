using MediatR;

namespace MyDisks.Services;

public interface ICommandHandler<in T, TResult> : IRequestHandler<T, TResult> where T : ICommand<TResult>
{
}
