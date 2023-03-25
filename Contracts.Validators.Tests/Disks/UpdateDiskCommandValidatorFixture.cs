using Contracts.Validators.Disks;
using FluentAssertions;
using MediatorExtension;
using MediatR;
using MyDisk.Contracts.Disks;
using MyDisk.Tests.Services;

namespace Contracts.Validators.Tests.Disks;

public class UpdateDiskCommandValidatorFixture
{
    [Theory]
    [AutoServiceData]
    public async Task ShouldThrowValidationException(string name)
    {
        // Arrange
        var command = new UpdateDiskCommand
            { Name = name, ReleaseDate = DateTime.Parse("30-07-23 16:01:55"), Id = null };
        var request = new Request<UpdateDiskCommand, Unit> { Value = command };

        // Act
        var act = await new UpdateDiskCommandValidator().ValidateAsync(request);

        // Assert
        act.IsValid.Should().BeFalse();
    }

    [Theory]
    [AutoServiceData]
    public async Task ShouldNotThrowValidationExceptionBecauseValidRequest(Request<UpdateDiskCommand, Unit> request)
    {
        // Act
        var act = await new UpdateDiskCommandValidator().ValidateAsync(request);

        // Assert
        act.IsValid.Should().BeTrue();
    }
}