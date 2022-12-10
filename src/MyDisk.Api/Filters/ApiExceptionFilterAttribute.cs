using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyDisk.Services.Common.Exceptions;

namespace MyDisk.Api.Filters
{
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;

        public ApiExceptionFilterAttribute()
        {
            _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
            {
                { typeof(ValidationException), HandleValidationException },
                { typeof(EntityNotFoundException), HandleEntityNotFoundException }
            };
        }

        public override void OnException(ExceptionContext context)
        {
            HandleException(context);
            base.OnException(context);
        }

        private void HandleException(ExceptionContext context)
        {
            Type type = context.Exception.GetType();
            if (_exceptionHandlers.ContainsKey(type))
            {
                _exceptionHandlers[type].Invoke(context);
                return;
            }
        }

        private void HandleValidationException(ExceptionContext context)
        {
            if (context.Exception is ValidationException exception)
            {
                var errors = exception.Errors;

                context.Result = new BadRequestObjectResult(errors.Select(c => c.ErrorMessage));

                context.ExceptionHandled = true;
            }
        }

        private void HandleEntityNotFoundException(ExceptionContext context)
        {
            if (context.Exception is EntityNotFoundException exception)
            {
                var error = exception.Message;

                context.Result = new BadRequestObjectResult(new List<string> { error });

                context.ExceptionHandled = true;
            }
        }
    }
}
