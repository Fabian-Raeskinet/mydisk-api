using System.Globalization;
using Contracts.Validators.Disks;
using FluentAssertions;
using MyDisk.Services.Disks;
using MyDisk.Tests.Services;

namespace Contracts.Validators.Tests.Disks;

public class CreateDiskCommandValidatorFixture
{
    [Theory]
    [InlineAutoServiceData(null, "30-07-23 16:01:55")]
    public async Task ShouldThrowValidationException(string name, string releaseDate)
    {
        // Arrange
        var request = new CreateDiskCommandRequest
        {
            Name = name,
            ReleaseDate = DateTime.ParseExact(releaseDate, "dd-MM-yy HH:mm:ss", CultureInfo.InvariantCulture)
        };

        // Act
        var act = await new CreateDiskCommandValidator().ValidateAsync(request);

        // Assert
        act.IsValid.Should().BeFalse();
    }

    [Theory]
    [AutoServiceData]
    public async Task ShouldNotThrowValidationExceptionBecauseValidRequest(CreateDiskCommandRequest request)
    {
        // Act
        var act = await new CreateDiskCommandValidator().ValidateAsync(request);

        // Assert
        act.IsValid.Should().BeTrue();
    }
}