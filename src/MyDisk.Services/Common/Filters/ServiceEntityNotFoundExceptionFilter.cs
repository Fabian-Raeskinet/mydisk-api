using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyDisk.Services.Common.Exceptions;
using MyDisk.Services.Common.Models;

namespace MyDisk.Services.Common.Filters
{
    public class ServiceEntityNotFoundExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception.GetType() != typeof(EntityNotFoundException))
                return;

            if (context.Exception is EntityNotFoundException entityNotFoundException)
            {
                context.Result = new BadRequestObjectResult(
                    new BadRequestResponse
                    {
                        Error = new BadRequestError { Messages = new List<string> { entityNotFoundException.Message}.ToArray() }
                    });
                context.ExceptionHandled = true;
            }
        }
    }
}
