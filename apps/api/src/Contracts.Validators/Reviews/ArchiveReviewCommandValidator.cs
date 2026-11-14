using FluentValidation;
using MyDisks.Services.Reviews;

namespace MyDisks.Contracts.Validators.Reviews;

public class ArchiveReviewCommandValidator : AbstractValidator<ArchiveReviewCommandRequest>
{
    public ArchiveReviewCommandValidator()
    {
        RuleFor(x => x.ReviewId).NotEmpty();
    }
}