using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyDisk.Services.Common.Models;

namespace MyDisk.Services.Filters
{
    public class ServiceValidationExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception.GetType() != typeof(ValidationException))
                return;

            if (context.Exception is ValidationException validationException)
            {
                context.Result = new BadRequestObjectResult(
                    new BadRequestResponse
                    {
                        Error = new BadRequestError { Messages = validationException.Errors.Select(c => c.ErrorMessage).ToArray() }
                    });
                context.ExceptionHandled = true;
            }
        }
    }
}
