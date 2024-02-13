using FluentValidation;
using MyDisks.Services.Reviews;

namespace MyDisks.Contracts.Validators.Reviews;

public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommandRequest>
{
    public CreateReviewCommandValidator()
    {
        RuleFor(x => x.DiskId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Content).NotEmpty();
        RuleFor(x => x.Note).GreaterThan(0).LessThan(5);
    }
}