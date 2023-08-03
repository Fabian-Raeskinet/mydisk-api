using MyDisks.Contracts.Validators.Disks;
using FluentAssertions;
using MyDisks.Contracts.Disks;
using MyDisks.Services.Disks;
using MyDisks.Tests.Services;

namespace MyDisks.Contracts.Validators.Tests.Disks;

public class DeleteDiskCommandValidatorFixture
{
    [Theory]
    [InlineAutoServiceData(null, "value")]
    [InlineAutoServiceData(null, null)]
    public async Task ShouldThrowValidationException(DeleteDiskByProperty property, string value)
    {
        // Arrange
        var request = new DeleteDiskCommandRequest { Property = property, Value = value };

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