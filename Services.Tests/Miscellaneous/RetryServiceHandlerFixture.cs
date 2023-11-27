using FluentAssertions;
using Moq;
using MyDisks.Services.Miscellaneous;
using MyDisks.Tests.Services;
using MyDisks.Tests.Utils;

namespace MyDisks.Services.Tests.Miscellaneous;

public class RetryServiceHandlerFixture
{
    public class HandleFixture
    {
        [Theory]
        [AutoServiceData]
        public async Task Should_ExecuteAsync
        (
            RetryServiceHandler sut,
            RetryServiceRequest request
        )
        {
            // Arrange
            Func<int, bool> func = r => r == 1;
            var test = () =>
            {
                var number = new Random().Next(1, 3);
                return Task.FromResult(number);
            };

            // Act
            await sut.Handle(request, default);

            // Assert
            sut.RetryService.AsMock()
                .Verify(x => x.ExecuteAsync(It.IsAny<Func<int, bool>>(), It.IsAny<Func<Task<int>>>()));

            sut.RetryService.AsMock()
                .Verify(x => x.ExecuteAsync<InvalidOperationException, int>(It.IsAny<Func<Task<int>>>()));
        }

        [Theory]
        [AutoServiceData]
        public async Task Should_ValidateCondition
        (
            RetryServiceHandler sut,
            RetryServiceRequest request
        )
        {
            // Act
            await sut.Handle(request, default);

            // Assert
            sut.RetryService.AsMock()
                .Verify(x => x.ExecuteAsync(
                    It.Is<Func<int, bool>>(f => f.Invoke(1)),
                    It.IsAny<Func<Task<int>>>()
                ), Times.Once);
        }

        [Theory]
        [AutoServiceData]
        public async Task Should_ExecuteAsync_Exception
        (
            RetryServiceHandler sut,
            RetryServiceRequest request
        )
        {
            // Act
            await sut.Handle(request, CancellationToken.None);

            // Assert
            sut.RetryService.AsMock()
                .Verify(
                    x => x.ExecuteAsync<InvalidOperationException, int>(
                        It.IsAny<Func<Task<int>>>()
                    ),
                    Times.Once);
        }
    }

    public class RetryWithInvalidOperationFuncFixture
    {
        [Theory]
        [AutoServiceData]
        public async Task Should_Use_RandomService(RetryServiceHandler sut)
        {
            // Act
            var act = await sut.RetryWithInvalidOperationFunc();
            
            // Assert
            sut.RandomService.AsMock()
                .Verify(_ => _.GetRandomValue(1, 3));
        }
        
        [Theory]
        [AutoServiceData]
        public async Task Should_Returns_Valid_Numbers(int returnedInt, RetryServiceHandler sut)
        {
            // Arrange
            sut.RandomService.AsMock()
                .Setup(_ => _.GetRandomValue(1, 3))
                .Returns(returnedInt);
            
            // Act
            var act = await sut.RetryWithInvalidOperationFunc();
            
            // Assert
            act.Should().Be(returnedInt);
        }
        
        [Theory]
        [AutoServiceData]
        public async Task Should_Throws_InvalidOperationException(RetryServiceHandler sut)
        {
            // Arrange
            sut.RandomService.AsMock()
                .Setup(_ => _.GetRandomValue(1, 3))
                .Returns(1);
            
            // Act
            var act = async () => await sut.RetryWithInvalidOperationFunc();
            
            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
        }
    }
    public class RetryWithPredicateFuncFixture
    {
        [Theory]
        [AutoServiceData]
        public async Task Should_Use_RandomService(RetryServiceHandler sut)
        {
            // Act
            var act = await sut.RetryWithPredicateFunc();
            
            // Assert
            sut.RandomService.AsMock()
                .Verify(_ => _.GetRandomValue(1, 3));
        }
        
        [Theory]
        [AutoServiceData]
        public async Task Should_Returns_Valid_Numbers(int returnedInt, RetryServiceHandler sut)
        {
            // Arrange
            sut.RandomService.AsMock()
                .Setup(_ => _.GetRandomValue(1, 3))
                .Returns(returnedInt);
            
            // Act
            var act = await sut.RetryWithPredicateFunc();
            
            // Assert
            act.Should().Be(returnedInt);
        }
    }
}