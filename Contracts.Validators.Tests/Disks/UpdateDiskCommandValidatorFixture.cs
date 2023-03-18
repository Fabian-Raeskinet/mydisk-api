using Contracts.Disks;
using Contracts.Validators.Disks;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Moq;
using MyDisk.Services.Behaviors;
using MyDisk.Tests.Services;

namespace Contracts.Validators.Tests.Disks;

public class UpdateDiskCommandValidatorFixture
{
    [Theory]
    [InlineAutoServiceData("", "value", "")]
    [InlineAutoServiceData(null, "value", "value")]
    public async Task ShouldThrowValidationException(string id, string name, string releaseDate)
    {
        // Arrange
        var request = new UpdateDiskCommand
        {
            Name = name,
            Id = id,
            ReleaseDate = releaseDate
        };
        var del = new Mock<RequestHandlerDelegate<DiskResponse>>();
        var sut = new ValidationBehaviour<UpdateDiskCommand, DiskResponse>(
            new List<IValidator<UpdateDiskCommand>> { new UpdateDiskCommandValidator() });
        
        // Act
        Func<Task> act = async () => await sut.Handle(request, del.Object, default);
        
        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
    
    [Theory]
    [AutoServiceData]
    public async Task ShouldNotThrowValidationExceptionBecauseValidRequest(UpdateDiskCommand request)
    {
        // Arrange
        var del = new Mock<RequestHandlerDelegate<DiskResponse>>();
        var sut = new ValidationBehaviour<UpdateDiskCommand, DiskResponse>(
            new List<IValidator<UpdateDiskCommand>> { new UpdateDiskCommandValidator() });
        
        // Act
        Func<Task> act = async () => await sut.Handle(request, del.Object, default);

        // Assert
        await act.Should().NotThrowAsync<ValidationException>();
    }
}