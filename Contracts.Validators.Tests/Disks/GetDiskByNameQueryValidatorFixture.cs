using Contracts.Disks;
using Contracts.Validators.Disks;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Moq;
using MyDisk.Services.Behaviors;
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
        var request = new GetDiskByNameQuery{Name = name};
        var del = new Mock<RequestHandlerDelegate<DiskResponse>>();
        var sut = new ValidationBehaviour<GetDiskByNameQuery, DiskResponse>(
            new List<IValidator<GetDiskByNameQuery>> { new GetDiskByNameQueryValidator() });
        
        // Act
        Func<Task> act = async () => await sut.Handle(request, del.Object, default);
        
        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
    
    [Theory]
    [AutoServiceData]
    public async Task ShouldNotThrowValidationExceptionBecauseValidRequest(GetDiskByNameQuery request)
    {
        var del = new Mock<RequestHandlerDelegate<DiskResponse>>();
        var sut = new ValidationBehaviour<GetDiskByNameQuery, DiskResponse>(
            new List<IValidator<GetDiskByNameQuery>> { new GetDiskByNameQueryValidator() });
        
        // Act
        Func<Task> act = async () => await sut.Handle(request, del.Object, default);

        // Assert
        await act.Should().NotThrowAsync<ValidationException>();
    }
}