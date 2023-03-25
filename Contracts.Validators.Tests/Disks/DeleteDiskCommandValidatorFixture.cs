using Contracts.Validators.Disks;
using FluentAssertions;
using MediatorExtension;
using MediatR;
using MyDisk.Contracts.Disks;
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
        var command = new DeleteDiskCommand { Property = property, Value = value };
        var request = new Request<DeleteDiskCommand, Unit> { Value = command };

        // Act
        var act = await new DeleteDiskCommandValidator().ValidateAsync(request);

        // Assert
        act.IsValid.Should().BeFalse();
    }

    [Theory]
    [AutoServiceData]
    public async Task ShouldNotThrowValidationException(Request<DeleteDiskCommand, Unit> request)
    {
        // Act
        var act = await new DeleteDiskCommandValidator().ValidateAsync(request);

        // Assert
        act.IsValid.Should().BeTrue();
    }
}