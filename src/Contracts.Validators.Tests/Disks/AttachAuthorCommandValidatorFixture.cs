using FluentAssertions;
using MyDisks.Contracts.Validators.Disks;
using MyDisks.Services.Disks;
using MyDisks.Tests.Services;

namespace MyDisks.Contracts.Validators.Tests.Disks;

public class AttachAuthorCommandValidatorFixture
{
    [Fact]
    public async Task ShouldThrowValidationException()
    {
        // Arrange
        var request = new AttachAuthorCommandRequest { AuthorId = Guid.Empty, DiskId = Guid.Empty };

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