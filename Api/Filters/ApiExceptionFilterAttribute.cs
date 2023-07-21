using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyDisk.Domain.Exceptions;

namespace MyDisk.Api.Filters;

public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
{
    public IDictionary<Type, Action<ExceptionContext>?> ExceptionHandlers { get; }

    public ApiExceptionFilterAttribute()
    {
        ExceptionHandlers = new Dictionary<Type, Action<ExceptionContext>?>
        {
            { typeof(ValidationException), HandleValidationException },
            { typeof(ObjectNotFoundException), HandleObjectyNotFoundException },
            { typeof(FormatException), HandleFormatException }
        };
    }

    public override void OnException(ExceptionContext context)
    {
        HandleException(context);
        base.OnException(context);
    }

    public void HandleException(ExceptionContext context)
    {
        var type = context.Exception.GetType();
        if (!ExceptionHandlers.TryGetValue(type, out var value)) return;
        value?.Invoke(context);
    }

    public void HandleValidationException(ExceptionContext context)
    {
        if (context.Exception is not ValidationException exception) return;
        var errors = exception.Errors;

        context.Result = new BadRequestObjectResult(errors.Select(c => c.ErrorMessage));

        context.ExceptionHandled = true;
    }

    public void HandleObjectyNotFoundException(ExceptionContext context)
    {
        if (context.Exception is not ObjectNotFoundException exception) return;
        var error = exception.Message;

        context.Result = new NotFoundObjectResult(new List<string> { error });

        context.ExceptionHandled = true;
    }

    public void HandleFormatException(ExceptionContext context)
    {
        if (context.Exception is not FormatException exception) return;
        var error = exception.Message;

        context.Result = new BadRequestObjectResult(new List<string> { error });

        context.ExceptionHandled = true;
    }
}