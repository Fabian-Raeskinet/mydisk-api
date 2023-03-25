using Contracts.Validators.Disks;
using FluentAssertions;
using MediatorExtension;
using MyDisk.Contracts.Disks;
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
        var request = new Request<GetDiskByNameQuery, DiskResponse>
        {
            Value = new GetDiskByNameQuery
                { Name = name }
        };

        // Act
        var act = await new GetDiskByNameQueryValidator().ValidateAsync(request);

        // Assert
        act.IsValid.Should().BeFalse();
    }

    [Theory]
    [AutoServiceData]
    public async Task ShouldNotThrowValidationException(Request<GetDiskByNameQuery, DiskResponse> request)
    {
        // Act
        var act = await new GetDiskByNameQueryValidator().ValidateAsync(request);

        // Assert
        act.IsValid.Should().BeTrue();
    }
}