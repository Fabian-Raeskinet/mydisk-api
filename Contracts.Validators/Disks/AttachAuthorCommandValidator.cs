using Contracts.Disks;
using FluentValidation;

namespace Contracts.Validators.Disks;

public class AttachAuthorCommandValidator : AbstractValidator<AttachAuthorCommand>
{
    public AttachAuthorCommandValidator()
    {
        RuleFor(x => x.AuthorId).NotNull().NotEmpty();
        RuleFor(x => x.DiskId).NotNull().NotEmpty();
    }
}