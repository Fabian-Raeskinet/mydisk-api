using MediatR;

namespace MyDisks.Services;

public interface ICommandHandler<in T> : IRequestHandler<T> where T : ICommand
{
    
}