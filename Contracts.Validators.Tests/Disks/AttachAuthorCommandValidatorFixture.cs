using Contracts.Disks;
using Contracts.Validators.Disks;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Moq;
using MyDisk.Services.Behaviors;
using MyDisk.Tests.Services;

namespace Contracts.Validators.Tests.Disks;

public class AttachAuthorCommandValidatorFixture
{
    [Theory]
    [InlineAutoServiceData("", "")]
    [InlineAutoServiceData("", "value")]
    [InlineAutoServiceData("value", "")]
    [InlineAutoServiceData(null, "value")]
    [InlineAutoServiceData(null, null)]
    public async Task ShouldThrowValidationException(string authorId, string diskId)
    {
        // Arrange
        var request = new AttachAuthorCommand { AuthorId = authorId, DiskId = diskId };
        var del = new Mock<RequestHandlerDelegate<DiskResponse>>();
        var sut = new ValidationBehaviour<AttachAuthorCommand, DiskResponse>(
            new List<IValidator<AttachAuthorCommand>> { new AttachAuthorCommandValidator() });

        // Act
        Func<Task> act = async () => await sut.Handle(request, del.Object, default);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Theory]
    [AutoServiceData]
    public async Task ShouldNotThrowValidationExceptionBecauseValidRequest(AttachAuthorCommand request)
    {
        // Arrange
        var del = new Mock<RequestHandlerDelegate<DiskResponse>>();
        var sut = new ValidationBehaviour<AttachAuthorCommand, DiskResponse>(
            new List<IValidator<AttachAuthorCommand>> { new AttachAuthorCommandValidator() });

        // Act
        Func<Task> act = async () => await sut.Handle(request, del.Object, default);

        // Assert
        await act.Should().NotThrowAsync<ValidationException>();
    }
}