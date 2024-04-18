using FluentAssertions;
using FluentValidation;
using MediatR;
using Moq;
using MyDisks.Contracts.Disks;
using MyDisks.Contracts.Validators.Disks;
using MyDisks.Services.Behaviors;
using MyDisks.Services.Disks;
using MyDisks.Tests.Services;

namespace MyDisks.Services.Tests.Behaviours;

public class ValidationBehaviourFixture
{
    [Fact]
    public async Task ShouldThrowValidationException()
    {
        // Arrange
        var request = new GetDiskByNameQueryRequest { Name = string.Empty };
        var del = new Mock<RequestHandlerDelegate<DiskResult>>();
        var sut = new ValidationBehaviour<GetDiskByNameQueryRequest, DiskResult>(
            new List<IValidator<GetDiskByNameQueryRequest>> { new GetDiskByNameQueryValidator() });

        // Act
        Func<Task> act = async () => await sut.Handle(request, del.Object, default);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task ShouldNotThrowValidationExceptionBecauseEmptyValidators()
    {
        // Arrange
        var request = new GetDiskByNameQueryRequest { Name = string.Empty };
        var del = new Mock<RequestHandlerDelegate<DiskResult>>();
        var sut = new ValidationBehaviour<GetDiskByNameQueryRequest, DiskResult>(
            new List<IValidator<GetDiskByNameQueryRequest>>());

        // Act
        Func<Task> act = async () => await sut.Handle(request, del.Object, default);

        // Assert
        await act.Should().NotThrowAsync<ValidationException>();
    }

    [Theory]
    [AutoServiceData]
    public async Task ShouldNotThrowValidationExceptionBecauseValidRequest(GetDiskByNameQueryRequest request)
    {
        // Arrange
        var del = new Mock<RequestHandlerDelegate<DiskResult>>();
        var sut = new ValidationBehaviour<GetDiskByNameQueryRequest, DiskResult>(
            new List<IValidator<GetDiskByNameQueryRequest>> { new GetDiskByNameQueryValidator() });

        // Act
        Func<Task> act = async () => await sut.Handle(request, del.Object, default);

        // Assert
        await act.Should().NotThrowAsync<ValidationException>();
    }
}