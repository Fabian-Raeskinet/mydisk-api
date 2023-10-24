using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.Filters;
using MyDisks.Api.Filters;
using MyDisks.Domain.Exceptions;
using MyDisks.Tests.Api;

namespace MyDisks.Api.Tests.Filters;

public class DummyException : Exception
{
}

public class ApiExceptionFilterAttributeFixture
{
    public class OnExceptionFixture
    {
        [Theory]
        [AutoApiData]
        public void Should_Not_Handle_Exception
        (
            [NoAutoProperties] ExceptionContext context,
            string exceptionMessage,
            ApiExceptionFilterAttribute sut
        )
        {
            // Arrange
            context.Exception = new Exception(exceptionMessage);

            // Act
            sut.OnException(context);

            // Assert
            context.ExceptionHandled.Should().BeFalse();
        }
    }

    public class HandleExceptionFixture
    {
        [Theory]
        [AutoApiData]
        public void Should_Not_Find_Action
        (
            [NoAutoProperties] ExceptionContext context,
            ApiExceptionFilterAttribute sut
        )
        {
            // Arrange
            // var invokingCount = 0;
            // var mockExceptionHandlers = new Dictionary<Type, Action<ExceptionContext>?>
            // {
            //     { typeof(ObjectNotFoundException), _ => invokingCount++ },
            // };
            //
            // sut.ExceptionHandlers = mockExceptionHandlers;
            //
            // context.Exception = new DummyException();
            // // sut.ExceptionHandlers[typeof(ObjectNotFoundException)] = _ => invokingCount++;
            //
            // // Act
            // sut.HandleException(context);
            //
            // // Assert
            // invokingCount.Should().Be(0);
            
            // Arrange
            var invokingCount = 0;
            context.Exception = new DummyException();
            sut.ExceptionHandlers[typeof(ObjectNotFoundException)] = _ => invokingCount++;
            
            // Act
            sut.HandleException(context);
            
            // Assert
            invokingCount.Should().Be(0);
        }
        
        [Theory]
        [AutoApiData]
        public void Should_Find_Action
        (
            [NoAutoProperties] ExceptionContext context,
            ApiExceptionFilterAttribute sut
        )
        {
            // Arrange
            var invokingCount = 0;
            context.Exception = new ObjectNotFoundException();
            sut.ExceptionHandlers[typeof(ObjectNotFoundException)] = _ => invokingCount++;
            
            // Act
            sut.HandleException(context);
            
            // Assert
            invokingCount.Should().Be(1);
        }
    }

    public class HandleValidationExceptionFixture
    {
        [Theory]
        [AutoApiData]
        public void Should_Not_Handle_DummyException
        (
            [NoAutoProperties] ExceptionContext context,
            ApiExceptionFilterAttribute sut
        )
        {
            // Arrange
            context.Exception = new DummyException();

            // Act
            sut.HandleValidationException(context);

            // Assert
            context.ExceptionHandled.Should().BeFalse();
        }

        [Theory]
        [AutoApiData]
        public void Should_Returns_BadRequestObjectResult
        (
            [NoAutoProperties] ExceptionContext context,
            string exceptionMessage,
            ApiExceptionFilterAttribute sut
        )
        {
            // Arrange

            context.Exception = new ValidationException(exceptionMessage);

            // Act
            sut.HandleValidationException(context);

            // Assert
            context.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Theory]
        [AutoApiData]
        public void Should_Have_ExceptionMessage
        (
            [NoAutoProperties] ExceptionContext context,
            IEnumerable<ValidationFailure> validationFailures,
            ApiExceptionFilterAttribute sut
        )
        {
            // Arrange
            var validationException = new ValidationException(validationFailures);
            context.Exception = validationException;

            // Act
            sut.HandleValidationException(context);

            // Assert
            var test = context.Result as BadRequestObjectResult;
            test!.Value.Should().BeEquivalentTo(validationException.Errors.Select(c => c.ErrorMessage));
        }

        [Theory]
        [AutoApiData]
        public void Should_Handle_Exception
        (
            [NoAutoProperties] ExceptionContext context,
            string exceptionMessage,
            ApiExceptionFilterAttribute sut
        )
        {
            // Arrange
            context.Exception = new ValidationException(exceptionMessage);

            // Act
            sut.HandleValidationException(context);

            // Assert
            context.ExceptionHandled.Should().BeTrue();
        }
    }

    public class HandleFormatExceptionFixture
    {
        [Theory]
        [AutoApiData]
        public void Should_Not_Handle_DummyException
        (
            [NoAutoProperties] ExceptionContext context,
            ApiExceptionFilterAttribute sut
        )
        {
            // Arrange
            context.Exception = new DummyException();

            // Act
            sut.HandleFormatException(context);

            // Assert
            context.ExceptionHandled.Should().BeFalse();
        }

        [Theory]
        [AutoApiData]
        public void Should_Returns_BadRequestObjectResult
        (
            [NoAutoProperties] ExceptionContext context,
            string exceptionMessage,
            ApiExceptionFilterAttribute sut
        )
        {
            // Arrange
            context.Exception = new FormatException(exceptionMessage);

            // Act
            sut.HandleFormatException(context);

            // Assert
            context.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Theory]
        [AutoApiData]
        public void Should_Have_ExceptionMessage
        (
            [NoAutoProperties] ExceptionContext context,
            string exceptionMessage,
            ApiExceptionFilterAttribute sut
        )
        {
            // Arrange
            context.Exception = new FormatException(exceptionMessage);

            // Act
            sut.HandleFormatException(context);

            // Assert
            context.Exception.Message.Should().Be(exceptionMessage);
        }

        [Theory]
        [AutoApiData]
        public void Should_Handle_Exception
        (
            [NoAutoProperties] ExceptionContext context,
            string exceptionMessage,
            ApiExceptionFilterAttribute sut
        )
        {
            // Arrange
            context.Exception = new FormatException(exceptionMessage);

            // Act
            sut.HandleFormatException(context);

            // Assert
            context.ExceptionHandled.Should().BeTrue();
        }
    }

    public class ObjectNotFoundExceptionFixture
    {
        [Theory]
        [AutoApiData]
        public void Should_Not_Handle_DummyException
        (
            [NoAutoProperties] ExceptionContext context,
            ApiExceptionFilterAttribute sut
        )
        {
            // Arrange
            context.Exception = new DummyException();

            // Act
            sut.HandleObjectyNotFoundException(context);

            // Assert
            context.ExceptionHandled.Should().BeFalse();
        }

        [Theory]
        [AutoApiData]
        public void Should_Returns_NotFoundObjectResult
        (
            [NoAutoProperties] ExceptionContext context,
            string exceptionMessage,
            ApiExceptionFilterAttribute sut
        )
        {
            // Arrange
            context.Exception = new ObjectNotFoundException(exceptionMessage);

            // Act
            sut.HandleObjectyNotFoundException(context);

            // Assert
            context.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Theory]
        [AutoApiData]
        public void Should_Have_ExceptionMessage
        (
            [NoAutoProperties] ExceptionContext context,
            string exceptionMessage,
            ApiExceptionFilterAttribute sut
        )
        {
            // Arrange
            context.Exception = new ObjectNotFoundException(exceptionMessage);

            // Act
            sut.HandleObjectyNotFoundException(context);

            // Assert
            context.Exception.Message.Should().Be(exceptionMessage);
        }

        [Theory]
        [AutoApiData]
        public void Should_Handle_Exception
        (
            [NoAutoProperties] ExceptionContext context,
            string exceptionMessage,
            ApiExceptionFilterAttribute sut
        )
        {
            // Arrange
            context.Exception = new ObjectNotFoundException(exceptionMessage);

            // Act
            sut.HandleObjectyNotFoundException(context);

            // Assert
            context.ExceptionHandled.Should().BeTrue();
        }
    }
}