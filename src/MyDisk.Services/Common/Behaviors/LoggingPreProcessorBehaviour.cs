using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace MyDisk.Services.Common.Behaviors;

public class LoggingPreProcessorBehaviour<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    private readonly ILogger _logger;

    public LoggingPreProcessorBehaviour(ILogger logger)
    {
        _logger = logger;
    }

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        _logger.LogInformation("MyDisk entering Request: {Name}", requestName);
    }
}