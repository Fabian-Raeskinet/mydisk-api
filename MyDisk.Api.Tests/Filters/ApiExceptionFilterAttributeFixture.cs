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
    public void ShouldReturnsNull(string exception)
    {
        // Arrange
        var context = new ExceptionContext(new ActionContext
        {
            HttpContext = new DefaultHttpContext(),
            RouteData = new RouteData(),
            ActionDescriptor = new ActionDescriptor()
        }, new List<IFilterMetadata>());
    
        context.Exception = new Exception(exception);
    
        // Act
        new ApiExceptionFilterAttribute().OnException(context);
    
        // Assert
        context.Result.Should().BeNull();
    }
    
    [Theory]
    [AutoApiData]
    public void ShouldReturnsBadRequestObjectResult(string exception)
    {
        // Arrange
        var context = new ExceptionContext(new ActionContext
        {
            HttpContext = new DefaultHttpContext(),
            RouteData = new RouteData(),
            ActionDescriptor = new ActionDescriptor()
        }, new List<IFilterMetadata>());
    
        context.Exception = new ValidationException(exception);
    
        // Act
        new ApiExceptionFilterAttribute().OnException(context);
    
        // Assert
        context.Result.Should().BeOfType<BadRequestObjectResult>();
        context.Exception.Message.Should().Be(exception);
    }
    
    [Theory]
    [AutoApiData]
    public void ShouldReturnsNotFoundObjectResult(string exception)
    {
        // Arrange
        var context = new ExceptionContext(new ActionContext
        {
            HttpContext = new DefaultHttpContext(),
            RouteData = new RouteData(),
            ActionDescriptor = new ActionDescriptor()
        }, new List<IFilterMetadata>());
    
        context.Exception = new ObjectNotFoundException(exception);
    
        // Act
        new ApiExceptionFilterAttribute().OnException(context);
    
        // Assert
        context.Result.Should().BeOfType<NotFoundObjectResult>();
        context.Exception.Message.Should().Be(exception);
    }
}