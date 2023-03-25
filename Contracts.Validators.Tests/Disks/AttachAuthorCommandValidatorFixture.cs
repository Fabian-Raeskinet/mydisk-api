using Contracts.Validators.Disks;
using FluentAssertions;
using MediatorExtension;
using MediatR;
using MyDisk.Contracts.Disks;
using MyDisk.Tests.Services;

namespace Contracts.Validators.Tests.Disks;

public class AttachAuthorCommandValidatorFixture
{
    [Theory]
    [AutoServiceData]
    public async Task ShouldThrowValidationException()
    {
        // Arrange
        var command = new AttachAuthorCommand { AuthorId = null, DiskId = null };
        var request = new Request<AttachAuthorCommand, Unit> { Value = command };

        // Act
        var act = await new AttachAuthorCommandValidator().ValidateAsync(request);

        // Assert
        act.IsValid.Should().BeFalse();
    }

    [Theory]
    [AutoServiceData]
    public async Task ShouldNotThrowValidationExceptionBecauseValidRequest(Request<AttachAuthorCommand, Unit> request)
    {
        // Act
        var act = await new AttachAuthorCommandValidator().ValidateAsync(request);

        // Assert
        act.IsValid.Should().BeTrue();
    }
}