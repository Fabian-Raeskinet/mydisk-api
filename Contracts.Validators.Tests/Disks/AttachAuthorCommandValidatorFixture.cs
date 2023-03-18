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
    
    //TODO
    // demander thomas/thierry ce qui est mieux comme type d'entrée pour les contrats
    [Theory]
    // [InlineAutoServiceData("", "")]
    // [InlineAutoServiceData("", "66a7609b-634e-4449-9ff8-7e757f98d86a")]
    // [InlineAutoServiceData("66a7609b-634e-4449-9ff8-7e757f98d86a", "")]
    [InlineAutoServiceData(null, "66a7609b-634e-4449-9ff8-7e757f98d86a")]
    [InlineAutoServiceData(null, null)]
    public async Task ShouldThrowValidationException(Guid authorId, Guid diskId)
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