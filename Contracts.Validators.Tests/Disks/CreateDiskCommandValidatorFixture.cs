using Contracts.Validators.Disks;
using FluentAssertions;
using MyDisk.Contracts.Disks;
using MyDisk.Tests.Services;

namespace Contracts.Validators.Tests.Disks;

public class CreateDiskCommandValidatorFixture
{
    [Theory]
    [InlineAutoServiceData(null, "30-07-23 16:01:55")]
    [InlineAutoServiceData(null, null)]
    public async Task ShouldThrowValidationException(string name, string? releaseDate)
    {
        // Arrange
        var request = new CreateDiskCommand
            { Name = name, ReleaseDate = releaseDate != null ? DateTime.Parse(releaseDate) : null };

        // Act
        var act = await new CreateDiskCommandValidator().ValidateAsync(request);

        // Assert
        act.IsValid.Should().BeFalse();
    }

    [Theory]
    [AutoServiceData]
    public async Task ShouldNotThrowValidationExceptionBecauseValidRequest(CreateDiskCommand request)
    {
        // Act
        var act = await new CreateDiskCommandValidator().ValidateAsync(request);

        // Assert
        act.IsValid.Should().BeTrue();
    }
}