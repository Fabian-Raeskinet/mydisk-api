using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace MyDisk.Services.Common.Behaviors
{
    public class LoggingPostProcessorBehaviour<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ILogger _logger;

        public LoggingPostProcessorBehaviour(ILogger logger)
        {
            _logger = logger;
        }

        public Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            _logger.LogInformation("MyDisk finished Request: {Name}", requestName);
            return Task.CompletedTask;
        }
    }
}
