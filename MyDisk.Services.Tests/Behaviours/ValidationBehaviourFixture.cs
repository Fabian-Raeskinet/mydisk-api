using Contracts.Validators.Disks;
using FluentAssertions;
using FluentValidation;
using MediatorExtension;
using MediatR;
using Moq;
using MyDisk.Contracts.Disks;
using MyDisk.Services.Behaviors;
using MyDisk.Tests.Services;

namespace MyDisk.Services.Tests.Behaviours;

public class ValidationBehaviourFixture
{
    [Fact]
    [AutoServiceData]
    public async Task ShouldThrowValidationException()
    {
        // Arrange
        var request = new Request<GetDiskByNameQuery, DiskResponse> { Value = new GetDiskByNameQuery() };
        var del = new Mock<RequestHandlerDelegate<DiskResponse>>();
        var sut = new ValidationBehaviour<Request<GetDiskByNameQuery, DiskResponse>, DiskResponse>(
            new List<IValidator<Request<GetDiskByNameQuery, DiskResponse>>> { new GetDiskByNameQueryValidator() });

        // Act
        Func<Task> act = async () => await sut.Handle(request, del.Object, default);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
    
    [Fact]
    [AutoServiceData]
    public async Task ShouldNotThrowValidationExceptionBecauseEmptyValidators()
    {
        // Arrange
        var request = new Request<GetDiskByNameQuery, DiskResponse> { Value = new GetDiskByNameQuery() };
        var del = new Mock<RequestHandlerDelegate<DiskResponse>>();
        var sut = new ValidationBehaviour<Request<GetDiskByNameQuery, DiskResponse>, DiskResponse>(
            new List<IValidator<Request<GetDiskByNameQuery, DiskResponse>>>());

        // Act
        Func<Task> act = async () => await sut.Handle(request, del.Object, default);

        // Assert
        await act.Should().NotThrowAsync<ValidationException>();
    }
    
    [Theory]
    [AutoServiceData]
    public async Task ShouldNotThrowValidationExceptionBecauseValidRequest(Request<GetDiskByNameQuery, DiskResponse> request)
    {
        // Arrange
        var del = new Mock<RequestHandlerDelegate<DiskResponse>>();
        var sut = new ValidationBehaviour<Request<GetDiskByNameQuery, DiskResponse>, DiskResponse>(
            new List<IValidator<Request<GetDiskByNameQuery, DiskResponse>>> { new GetDiskByNameQueryValidator() });

        // Act
        Func<Task> act = async () => await sut.Handle(request, del.Object, default);

        // Assert
        await act.Should().NotThrowAsync<ValidationException>();
    }
}