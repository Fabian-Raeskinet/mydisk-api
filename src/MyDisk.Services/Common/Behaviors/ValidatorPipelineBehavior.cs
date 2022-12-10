using FluentValidation;
using MediatR;

namespace MyDisk.Services.Common.Behaviors
{
    public class ValidatorPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidatorPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var validationFailures = _validators
               .Select(validator => validator.Validate(request))
               .SelectMany(validationResult => validationResult.Errors)
               .Where(validationFailure => validationFailure != null)
               .ToList();

            if (validationFailures.Any())
            {
                throw new ValidationException(validationFailures);
            }

            return await next();
        }
    }
}
