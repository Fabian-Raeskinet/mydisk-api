using System.Globalization;
using Contracts.Validators.Disks;
using FluentAssertions;
using MyDisk.Services.Disks;
using MyDisk.Tests.Services;

namespace Contracts.Validators.Tests.Disks;

public class UpdateDiskCommandValidatorFixture
{
    [Theory]
    [AutoServiceData]
    public async Task ShouldThrowValidationException(string name)
    {
        // Arrange
        var request = new UpdateDiskCommandRequest
            { Name = name, ReleaseDate = DateTime.Parse("30-07-23 16:01:55", CultureInfo.InvariantCulture), Id = Guid.Empty };
    
        // Act
        var act = await new UpdateDiskCommandValidator().ValidateAsync(request);
    
        // Assert
        act.IsValid.Should().BeFalse();
    }

    [Theory]
    [AutoServiceData]
    public async Task ShouldNotThrowValidationExceptionBecauseValidRequest(UpdateDiskCommandRequest request)
    {
        // Act
        var act = await new UpdateDiskCommandValidator().ValidateAsync(request);

        // Assert
        act.IsValid.Should().BeTrue();
    }
}