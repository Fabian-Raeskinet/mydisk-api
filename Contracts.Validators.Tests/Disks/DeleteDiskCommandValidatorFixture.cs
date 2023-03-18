using Contracts.Disks;
using Contracts.Validators.Disks;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Moq;
using MyDisk.Services.Behaviors;
using MyDisk.Tests.Services;

namespace Contracts.Validators.Tests.Disks;

public class DeleteDiskCommandValidatorFixture
{
    [Theory]
    [InlineAutoServiceData(null, "value")]
    [InlineAutoServiceData(null, null)]
    public async Task ShouldThrowValidationException(DeleteDiskByProperty property, string value)
    {
        // Arrange
        var request = new DeleteDiskCommand{Property = property, Value = value};
        var del = new Mock<RequestHandlerDelegate<Unit>>();
        var sut = new ValidationBehaviour<DeleteDiskCommand, Unit>(
            new List<IValidator<DeleteDiskCommand>> { new DeleteDiskCommandValidator() });
        
        // Act
        Func<Task> act = async () => await sut.Handle(request, del.Object, default);
        
        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
    
    [Theory]
    [AutoServiceData]
    public async Task ShouldNotThrowValidationExceptionBecauseValidRequest(DeleteDiskCommand request)
    {
        // Arrange
        var del = new Mock<RequestHandlerDelegate<Unit>>();
        var sut = new ValidationBehaviour<DeleteDiskCommand, Unit>(
            new List<IValidator<DeleteDiskCommand>> { new DeleteDiskCommandValidator() });
        
        // Act
        Func<Task> act = async () => await sut.Handle(request, del.Object, default);

        // Assert
        await act.Should().NotThrowAsync<ValidationException>();
    }
}