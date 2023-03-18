using System.Linq.Expressions;
using Contracts.Disks;
using Contracts.Validators.Disks;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Moq;
using MyDisk.Domain.Entities;
using MyDisk.Services.Behaviors;
using MyDisk.Services.Disks;
using MyDisk.Tests.Services;
using MyDisk.Tests.Utils;

namespace MyDisk.Services.Tests.Disks;

public class GetDiskByNameQueryHandlerTestsShould
{
    [Theory]
    [AutoServiceData]
    public async Task FindDisk
    (
        GetDiskByNameQueryHandler sut,
        Disk disk
    )
    {
        // Arrange
        sut.DiskRepository.AsMock()
            .Setup(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()))
            .ReturnsAsync(disk);

        // Act
        var act = await sut.Handle(It.IsAny<GetDiskByNameQuery>(), It.IsAny<CancellationToken>());

        // Assert
        sut.Mapper.AsMock()
            .Verify(x => x.Map<DiskResponse>(disk), Times.Once);
        act.Should().NotBeNull();
    }

    [Theory]
    [AutoServiceData]
    public async Task NotFindDisk
    (
        GetDiskByNameQueryHandler sut
    )
    {
        // Arrange
        sut.DiskRepository.AsMock()
            .Setup(x => x.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()))
            .ReturnsAsync(() => null);

        // Act
        var act = await sut.Handle(It.IsAny<GetDiskByNameQuery>(), It.IsAny<CancellationToken>());

        // Assert
        sut.Mapper.AsMock()
            .Verify(x => x.Map<DiskResponse>(It.IsAny<Disk>()), Times.Never);
        act.Should().BeNull();
    }
}
public class ValidationBehaviourFixture
{

    
    [Theory]
    [AutoServiceData]
    public async Task ShouldNotThrowValidationExceptionBecauseNoValidators(GetDiskByNameQuery request)
    {
        var del = new Mock<RequestHandlerDelegate<DiskResponse>>();
        var sut = new ValidationBehaviour<GetDiskByNameQuery, DiskResponse>(
            new List<IValidator<GetDiskByNameQuery>> { });
        
        //Act
        Func<Task> act = async () => await sut.Handle(request, del.Object, default);

        await act.Should().NotThrowAsync<ValidationException>();
    }
}