using Contracts.Disks;
using Contracts.Validators.Disks;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Moq;
using MyDisk.Services.Behaviors;
using MyDisk.Tests.Services;

namespace Contracts.Validators.Tests.Disks;

public class CreateDiskCommandValidatorFixture
{
    [Theory]
    [InlineAutoServiceData("", "")]
    [InlineAutoServiceData("", "value")]
    [InlineAutoServiceData("value", "")]
    [InlineAutoServiceData(null, "value")]
    [InlineAutoServiceData(null, null)]
    public async Task ShouldThrowValidationException(string name, string releaseDate)
    {
        // Arrange
        var request = new CreateDiskCommand { Name = name, ReleaseDate = releaseDate };
        var del = new Mock<RequestHandlerDelegate<Guid>>();
        var sut = new ValidationBehaviour<CreateDiskCommand, Guid>(
            new List<IValidator<CreateDiskCommand>> { new CreateDiskCommandValidator() });

        // Act
        Func<Task> act = async () => await sut.Handle(request, del.Object, default);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Theory]
    [AutoServiceData]
    public async Task ShouldNotThrowValidationExceptionBecauseValidRequest(CreateDiskCommand request)
    {
        // Arrange
        var del = new Mock<RequestHandlerDelegate<Guid>>();
        var sut = new ValidationBehaviour<CreateDiskCommand, Guid>(
            new List<IValidator<CreateDiskCommand>> { new CreateDiskCommandValidator() });

        // Act
        Func<Task> act = async () => await sut.Handle(request, del.Object, default);

        // Assert
        await act.Should().NotThrowAsync<ValidationException>();
    }
}