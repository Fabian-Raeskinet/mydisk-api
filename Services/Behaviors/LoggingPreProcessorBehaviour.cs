using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace MyDisk.Services.Behaviors;

public class LoggingPreProcessorBehaviour<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    public ILogger<TRequest> Logger { get; }

    public LoggingPreProcessorBehaviour(ILogger<TRequest> logger)
    {
        Logger = logger;
    }

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        Logger.LogInformation("MyDisk entering Request: {Name}", requestName);
        await Task.CompletedTask;
    }
}