using FluentValidation;
using MediatorExtension.Disks;

namespace Contracts.Validators.Disks;

public class AttachAuthorCommandValidator : AbstractValidator<AttachAuthorCommandRequest>
{
    public AttachAuthorCommandValidator()
    {
        RuleFor(x => x.AuthorId).NotNull().NotEmpty();
        RuleFor(x => x.DiskId).NotNull().NotEmpty();
    }
}