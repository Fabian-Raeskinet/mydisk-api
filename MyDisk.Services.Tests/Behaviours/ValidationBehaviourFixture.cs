using Contracts.Validators.Disks;
using FluentAssertions;
using FluentValidation;
using MediatorExtension;
using MediatorExtension.Disks;
using MediatR;
using Moq;
using MyDisk.Contracts.Disks;
using MyDisk.Services.Behaviors;
using MyDisk.Tests.Services;

namespace MyDisk.Services.Tests.Behaviours;

public class ValidationBehaviourFixture
{
    [Fact]
    public async Task ShouldThrowValidationException()
    {
        // Arrange
        var request = new GetDiskByNameQueryRequest();
        var del = new Mock<RequestHandlerDelegate<DiskResponse>>();
        var sut = new ValidationBehaviour<GetDiskByNameQueryRequest, DiskResponse>(
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
        var request = new GetDiskByNameQueryRequest();
        var del = new Mock<RequestHandlerDelegate<DiskResponse>>();
        var sut = new ValidationBehaviour<GetDiskByNameQueryRequest, DiskResponse>(
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
        var del = new Mock<RequestHandlerDelegate<DiskResponse>>();
        var sut = new ValidationBehaviour<GetDiskByNameQueryRequest, DiskResponse>(
            new List<IValidator<GetDiskByNameQueryRequest>> { new GetDiskByNameQueryValidator() });

        // Act
        Func<Task> act = async () => await sut.Handle(request, del.Object, default);

        // Assert
        await act.Should().NotThrowAsync<ValidationException>();
    }
}