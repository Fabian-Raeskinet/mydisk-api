using FluentValidation.TestHelper;
using MyDisks.Contracts.Validators.Reviews;
using MyDisks.Services.Reviews;
using MyDisks.Tests.Services;

namespace MyDisks.Contracts.Validators.Tests.Reviews;

public class ArchiveReviewCommandValidatorFixture
{
    [Theory]
    [AutoServiceData]
    public void Should_Fail_When_DiskId_Is_Empty
    (
        ArchiveReviewCommandRequest request,
        ArchiveReviewCommandValidator sut
    )
    {
        // Arrange
        request.ReviewId = Guid.Empty;

        // Act
        var act = sut.TestValidate(request);

        // Assert
        act.ShouldHaveValidationErrorFor(x => x.ReviewId);
    }

    [Theory]
    [AutoServiceData]
    public void Should_Pass_When_All_Validation_Passes
    (
        ArchiveReviewCommandRequest request,
        ArchiveReviewCommandValidator sut
    )
    {
        // Act
        var act = sut.TestValidate(request);

        // Assert
        act.ShouldNotHaveAnyValidationErrors();
    }
}