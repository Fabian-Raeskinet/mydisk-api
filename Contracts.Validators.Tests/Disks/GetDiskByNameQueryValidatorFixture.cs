using Contracts.Validators.Disks;
using FluentAssertions;
using MediatorExtension.Disks;
using MyDisk.Tests.Services;

namespace Contracts.Validators.Tests.Disks;

public class GetDiskByNameQueryValidatorFixture
{
    [Theory]
    [InlineAutoServiceData("")]
    [InlineAutoServiceData(null)]
    public async Task ShouldThrowValidationException(string name)
    {
        // Arrange
        var request = new GetDiskByNameQueryRequest
        {
            Name = name
        };

        // Act
        var act = await new GetDiskByNameQueryValidator().ValidateAsync(request);

        // Assert
        act.IsValid.Should().BeFalse();
    }

    [Theory]
    [AutoServiceData]
    public async Task ShouldNotThrowValidationException(GetDiskByNameQueryRequest request)
    {
        // Act
        var act = await new GetDiskByNameQueryValidator().ValidateAsync(request);

        // Assert
        act.IsValid.Should().BeTrue();
    }
}