using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace MyDisks.Services.Behaviors;

public class LoggingPreProcessorBehaviour<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    public LoggingPreProcessorBehaviour(ILogger<TRequest> logger)
    {
        Logger = logger;
    }

    public ILogger<TRequest> Logger { get; }

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        Logger.LogInformation("MyDisk entering Request: {Name}", requestName);
        await Task.CompletedTask;
    }
}