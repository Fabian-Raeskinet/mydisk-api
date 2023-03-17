using MediatR;
using MyDisk.Services.Disks.DTOs;
using MyDisk.Services.Disks.Requests;
using MyDisk.Tests.Api;

namespace MyDisk.Api.Tests.Controllers;

public class GetAllDisksShould
{
    [Theory, AutoApiData]
    public async void ReturnsOkResultTest([Frozen] Mock<IMediator> mediator, [NoAutoProperties] DiskController sut)
    {
        mediator.Setup(x => x.Send(It.IsAny<GetAllDisksRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DiskEntity>());
        var result = await sut.GetAllDisks();
        result.Should().BeOfType<OkObjectResult>();
        mediator.Verify(x => x.Send(It.IsAny<GetAllDisksRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class GetDiskByNameShould
{
    [Theory, AutoApiData]
    public async void ReturnsOkResult([Frozen] Mock<IMediator> mediator, [NoAutoProperties] DiskController sut,
        DiskResponse response)
    {
        mediator.Setup(x => x.Send(It.IsAny<GetDiskByNameRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);
        var result = await sut.GetByName(It.IsAny<string>());
        result.Should().BeOfType<OkObjectResult>();
        mediator.Verify(x => x.Send(It.IsAny<GetDiskByNameRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory, AutoApiData]
    public async void ReturnsNotFound([Frozen] Mock<IMediator> mediator, [NoAutoProperties] DiskController sut)
    {
        DiskResponse? response = null;
        mediator.Setup(x => x.Send(It.IsAny<GetDiskByNameRequest>(), It.IsAny<CancellationToken>()))!
            .ReturnsAsync(response);
        var result = await sut.GetByName(It.IsAny<string>());
        result.Should().BeOfType<NotFoundResult>();
        mediator.Verify(x => x.Send(It.IsAny<GetDiskByNameRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

//     [Theory, AutoApiData]
//     public async void ReturnsBadRequest([NoAutoProperties] DiskController sut)
//     {
//         string? name = null;
//         var result = await sut.GetByName(name);
//         result.Should().BeOfType<BadRequestObjectResult>();
//
//         var request = new GetDiskByNameRequest { Name = null };
//         
//         var commandHandler = new GetDiskByNameQueryHandler();
//         var validationBehaviour =
//             new ValidationBehaviour<GetDiskByNameRequest, DiskResponse>(new List<IValidator<GetDiskByNameRequest>>());
//         
//         await Assert.ThrowsAsync<ValidationException>(() =>
//             validationBehaviour.Handle(request, () =>
//             {
//                 return
//             }, It.IsAny<CancellationToken>()));
//         
//         
//         https://stackoverflow.com/questions/60687927/unit-testing-validation-through-mediatr-pipelinebehavior
//     }
// }

    // [Theory]
    // [AutoDomainData]
    // public async Task ShouldThrowException(
    //     Mock<ICurrentUserService> currentUserServiceMock,
    //     AssociateStakeholderToAccountRequest request
    // )
    // {
    //     // Arrange
    //     request.SignInEmail = "certainEmail";
    //     var claimsEmail = "otherEmail";
    //         
    //     currentUserServiceMock
    //         .Setup(x => x.Email)
    //         .Returns(claimsEmail);
    //     var del = new Mock<RequestHandlerDelegate<AssociateStakeholderToAccountNotification?>>();
    //     var sut = new AuthorizationPipelineBehaviour<IRequest<AssociateStakeholderToAccountNotification?>, AssociateStakeholderToAccountNotification?>(currentUserServiceMock.Object);
    //         
    //     // Act
    //     Func<Task> act = async () => await sut.Handle(request, del.Object, default);
    //         
    //     //Assert
    //     await act.Should().ThrowAsync<ArgumentException>();
    // }
    
    // [Theory]
    // [AutoDomainData]
    // public async Task ReturnsBadRequest
    // (
    //     [Frozen] Mock<IMediator> mediator
    // )
    // {
    //     var request = new EanValidationRequest();
    //     var del = new Mock<RequestHandlerDelegate<Unit>>();
    //     var sut = new ValidatorPipelineBehavior<EanValidationRequest, Unit>(
    //         new List<IValidator<EanValidationRequest>> { new EanValidationRequestValidator() });
    //
    //     //Act
    //     Func<Task> act = async () => await sut.Handle(request, del.Object, default);
    //
    //     await act.Should().ThrowAsync<ValidationException>();
    // }
    
    
    
    
    
    
    
    
    
    
    // var request = new EanValidationRequest();
    // var del = new Mock<RequestHandlerDelegate<Unit>>();
    // var sut = new ValidatorPipelineBehavior<EanValidationRequest, Unit>(
    //     new List<IValidator<EanValidationRequest>> { new EanValidationRequestValidator() });
    //         
    // //Act
    // Func<Task> act = async () => await sut.Handle(request, del.Object, default);
    //         
    // await act.Should().ThrowAsync<ValidationException>();
    //         
    //         
    // // Arrange
    // var context = new ExceptionContext(new ActionContext
    // {
    //     HttpContext = new DefaultHttpContext(),
    //     RouteData = new RouteData(),
    //     ActionDescriptor = new ActionDescriptor()
    // }, new List<IFilterMetadata>());
    //
    // context.Exception = new ValidationException("test");
    //
    // // Act
    // new ApiExceptionFilterAttribute().OnException(context);
    //
    // // Assert
    // context.Result.Should().BeOfType<BadRequestObjectResult>();

    /*  [Theory, AutoApiData]
      public async void ReturnsBadRequest
      (
          [Frozen] Mock<IMediator> mediator,
          // [Frozen][NoAutoProperties] Mock<ValidationBehaviour<GetDiskByNameRequest,DiskResponse>> pipeline,
          [NoAutoProperties] DiskController sut,
          DiskResponse response
      )
      {
          string? name = null;
          // pipeline.Setup(x => x.Handle(
          //         It.IsAny<GetDiskByNameRequest>(),
          //         It.IsAny<RequestHandlerDelegate<DiskResponse>>(),
          //         It.IsAny<CancellationToken>()))
          //     .Throws(new ValidationException(new List<ValidationFailure>()));
          
          var validationBehavior = new ValidationBehaviour<GetDiskByNameRequest, DiskResponse>(new List<GetDiskByNameRequestValidator>()
          {
              new GetDiskByNameRequestValidator()
          });
          
          mediator.Setup(x => x.Send(It.IsAny<GetDiskByNameRequest>(), It.IsAny<CancellationToken>()))
              .ReturnsAsync(response)
              .Verifiable();
  
          var result = await sut.GetByName(name);
          
          mediator.Verify();
          result.Should().BeOfType<BadRequestObjectResult>();
      }
  }*/

    public class CreateDiskShould
    {
        [Theory, AutoApiData]
        public async Task ReturnsOkResult([Frozen] Mock<IMediator> mediator, [NoAutoProperties] DiskController sut,
            Guid diskId)
        {
            mediator.Setup(x => x.Send(It.IsAny<CreateDiskRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(diskId);
            var result = await sut.CreateDisk(It.IsAny<CreateDiskRequest>());
            result.Should().BeOfType<OkObjectResult>();
            mediator.Verify(x => x.Send(It.IsAny<CreateDiskRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        // [Theory, AutoApiData]
        // public async Task ReturnsBadRequest([NoAutoProperties] DiskController sut, [NoAutoProperties] CreateDiskRequest request)
        // {
        //     var result = await sut.CreateDisk(request);
        //     result.Should().BeOfType<BadRequestObjectResult>();
        // }
    }

    public class AttachAuthorShould
    {
        [Theory, AutoApiData]
        public async Task ReturnsOkResult([Frozen] Mock<IMediator> mediator, [NoAutoProperties] DiskController sut)
        {
            var result = await sut.AttachAuthor(It.IsAny<AttachAuthorRequest>());
            result.Should().BeOfType<OkObjectResult>();
            mediator.Verify(x => x.Send(It.IsAny<AttachAuthorRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        // [Theory, AutoApiData]
        // public async Task ReturnsNotFoundResult(
        //     [Frozen] Mock<IDiskRepository> repository,
        //     [NoAutoProperties] DiskController sut)
        // {
        //     repository.Setup(x => x.GetDiskByFilter(It.IsAny<Func<Disk, bool>>())).Returns(() => null);
        //     var result = await sut.AttachAuthor(It.IsAny<AttachAuthorRequest>());
        //     result.Should().BeOfType<NotFoundObjectResult>();
        // }
        //
        // [Theory, AutoApiData]
        // public async Task ReturnsBadRequest([NoAutoProperties] DiskController sut, [NoAutoProperties] AttachAuthorRequest request)
        // {
        //     var result = await sut.AttachAuthor(request);
        //     result.Should().BeOfType<BadRequestObjectResult>();
        // }
    }

    public class DeleteDiskByIdShould
    {
        [Theory, AutoApiData]
        public async Task ReturnsNoContentResult([Frozen] Mock<IMediator> mediator,
            [NoAutoProperties] DiskController sut)
        {
            mediator.Setup(x => x.Send(It.IsAny<DeleteDiskRequest>(), It.IsAny<CancellationToken>()))
                .Verifiable();
            var result = await sut.DeleteDiskById(It.IsAny<Guid>());
            result.Should().BeOfType<NoContentResult>();
            mediator.Verify(x => x.Send(It.IsAny<DeleteDiskRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }

    public class DeleteDiskByNameShould
    {
        [Theory, AutoApiData]
        public async Task ReturnsNoContentResult([Frozen] Mock<IMediator> mediator,
            [NoAutoProperties] DiskController sut)
        {
            mediator.Setup(x => x.Send(It.IsAny<DeleteDiskRequest>(), It.IsAny<CancellationToken>()))
                .Verifiable();
            var result = await sut.DeleteDiskByName(It.IsAny<string>());
            result.Should().BeOfType<NoContentResult>();
            mediator.Verify(x => x.Send(It.IsAny<DeleteDiskRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }

    public class UpdateDiskShould
    {
        [Theory, AutoApiData]
        public async Task ReturnsOkResult([Frozen] Mock<IMediator> mediator, [NoAutoProperties] DiskController sut)
        {
            mediator.Setup(x => x.Send(It.IsAny<UpdateDiskRequest>(), It.IsAny<CancellationToken>()))
                .Verifiable();
            var result = await sut.UpdateDisk(It.IsAny<UpdateDiskRequest>());
            result.Should().BeOfType<OkObjectResult>();
            mediator.Verify(x => x.Send(It.IsAny<UpdateDiskRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}