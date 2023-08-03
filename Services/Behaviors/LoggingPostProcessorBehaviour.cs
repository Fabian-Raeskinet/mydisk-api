using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace MyDisks.Services.Behaviors;

public class LoggingPostProcessorBehaviour<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public ILogger<TRequest> Logger { get; set; }

    public LoggingPostProcessorBehaviour(ILogger<TRequest> logger)
    {
        Logger = logger;
    }

    public Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        Logger.LogInformation("MyDisk finished Request: {Name}", requestName);
        return Task.CompletedTask;
    }
}