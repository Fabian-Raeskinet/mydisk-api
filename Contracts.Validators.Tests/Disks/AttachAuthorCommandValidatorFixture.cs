using Contracts.Validators.Disks;
using FluentAssertions;
using MyDisk.Contracts.Disks;
using MyDisk.Services.Disks;
using MyDisk.Tests.Services;

namespace Contracts.Validators.Tests.Disks;

public class AttachAuthorCommandValidatorFixture
{
    [Fact]
    public async Task ShouldThrowValidationException()
    {
        // Arrange
        var request = new AttachAuthorCommandRequest { AuthorId = null, DiskId = null };

        // Act
        var act = await new AttachAuthorCommandValidator().ValidateAsync(request);

        // Assert
        act.IsValid.Should().BeFalse();
    }

    [Theory]
    [AutoServiceData]
    public async Task ShouldNotThrowValidationExceptionBecauseValidRequest(AttachAuthorCommandRequest request)
    {
        // Act
        var act = await new AttachAuthorCommandValidator().ValidateAsync(request);

        // Assert
        act.IsValid.Should().BeTrue();
    }
}