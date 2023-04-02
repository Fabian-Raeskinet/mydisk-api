using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using MyDisk.Api.Filters;
using MyDisk.Domain.Exceptions;
using MyDisk.Tests.Api;

namespace MyDisk.Api.Tests.Filters;

public class ApiExceptionFilterAttributeFixture
{
    [Theory]
    [AutoApiData]
    public void ShouldReturnsNull([NoAutoProperties] ExceptionContext context, string exception)
    {
        // Arrange
        context.Exception = new Exception(exception);

        // Act
        new ApiExceptionFilterAttribute().OnException(context);

        // Assert
        context.Result.Should().BeNull();
    }

    public class ValidationExceptionFixture
    {
        [Theory]
        [AutoApiData]
        public void ShouldReturnsBadRequestObjectResult([NoAutoProperties] ExceptionContext context, string exception)
        {
            // Arrange
            context.Exception = new ValidationException(exception);

            // Act
            new ApiExceptionFilterAttribute().OnException(context);

            // Assert
            context.Result.Should().BeOfType<BadRequestObjectResult>();
            context.Exception.Message.Should().Be(exception);
        }
    }
    
    public class FormatExceptionFixture
    {
        [Theory]
        [AutoApiData]
        public void ShouldReturnsBadRequestObjectResult([NoAutoProperties] ExceptionContext context, string exception)
        {
            // Arrange
            context.Exception = new FormatException(exception);

            // Act
            new ApiExceptionFilterAttribute().OnException(context);

            // Assert
            context.Result.Should().BeOfType<BadRequestObjectResult>();
            context.Exception.Message.Should().Be(exception);
        }
    }
    
    public class ObjectNotFoundExceptionFixture
    {
        [Theory]
        [AutoApiData]
        public void ShouldReturnsBadRequestObjectResult([NoAutoProperties] ExceptionContext context, string exception)
        {
            // Arrange
            context.Exception = new ObjectNotFoundException(exception);

            // Act
            new ApiExceptionFilterAttribute().OnException(context);

            // Assert
            context.Result.Should().BeOfType<NotFoundObjectResult>();
            context.Exception.Message.Should().Be(exception);
        }
    }
}