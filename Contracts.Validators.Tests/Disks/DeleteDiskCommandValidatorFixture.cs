using FluentAssertions;
using MyDisks.Contracts.Disks;
using MyDisks.Contracts.Validators.Disks;
using MyDisks.Services.Disks;
using MyDisks.Tests.Services;

namespace MyDisks.Contracts.Validators.Tests.Disks;

public class DeleteDiskCommandValidatorFixture
{
    [Fact]
    public async Task ShouldThrowValidationException()
    {
        // Arrange
        var request = new DeleteDiskCommandRequest { DiskId = Guid.Empty};

        // Act
        var act = await new DeleteDiskCommandValidator().ValidateAsync(request);

        // Assert
        act.IsValid.Should().BeFalse();
    }

    [Theory]
    [AutoServiceData]
    public async Task ShouldNotThrowValidationException(DeleteDiskCommandRequest request)
    {
        // Act
        var act = await new DeleteDiskCommandValidator().ValidateAsync(request);

        // Assert
        act.IsValid.Should().BeTrue();
    }
}