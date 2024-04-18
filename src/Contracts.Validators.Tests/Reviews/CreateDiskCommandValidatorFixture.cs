using FluentValidation.TestHelper;
using MyDisks.Contracts.Validators.Reviews;
using MyDisks.Services.Reviews;
using MyDisks.Tests.Services;

namespace MyDisks.Contracts.Validators.Tests.Reviews;

public class CreateDiskCommandValidatorFixture
{
    [Theory]
    [AutoServiceData]
    public void Should_Fail_When_DiskId_Is_Empty(CreateReviewCommandRequest request, CreateReviewCommandValidator sut)
    {
        // Arrange
        request.DiskId = Guid.Empty;

        // Act
        var act = sut.TestValidate(request);

        // Assert
        act.ShouldHaveValidationErrorFor(x => x.DiskId);
    }

    [Theory]
    [AutoServiceData]
    public void Should_Fail_When_Title_Is_Empty(CreateReviewCommandRequest request, CreateReviewCommandValidator sut)
    {
        // Arrange
        request.Title = string.Empty;

        // Act
        var act = sut.TestValidate(request);

        // Assert
        act.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Theory]
    [AutoServiceData]
    public void Should_Fail_When_Content_Is_Empty(CreateReviewCommandRequest request, CreateReviewCommandValidator sut)
    {
        // Arrange
        request.Content = string.Empty;

        // Act
        var act = sut.TestValidate(request);

        // Assert
        act.ShouldHaveValidationErrorFor(x => x.Content);
    }

    [Theory]
    [AutoServiceData]
    public void Should_Fail_When_Note_Is_Less_Than_Zero(CreateReviewCommandRequest request,
        CreateReviewCommandValidator sut)
    {
        // Arrange
        request.Note = -1;

        // Act
        var act = sut.TestValidate(request);

        // Assert
        act.ShouldHaveValidationErrorFor(x => x.Note);
    }

    [Theory]
    [AutoServiceData]
    public void Should_Fail_When_Note_Is_Greater_Than_Five(CreateReviewCommandRequest request,
        CreateReviewCommandValidator sut)
    {
        // Arrange
        request.Note = 6;

        // Act
        var act = sut.TestValidate(request);

        // Assert
        act.ShouldHaveValidationErrorFor(x => x.Note);
    }

    [Theory]
    [AutoServiceData]
    public void Should_Pass_When_All_Validation_Passes(CreateReviewCommandRequest request,
        CreateReviewCommandValidator sut)
    {
        // Act
        var act = sut.TestValidate(request);

        // Assert
        act.ShouldNotHaveAnyValidationErrors();
    }
}