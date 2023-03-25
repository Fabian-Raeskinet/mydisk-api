using FluentValidation;
using MediatorExtension.Disks;

namespace Contracts.Validators.Disks;

public class AttachAuthorCommandValidator : AbstractValidator<AttachAuthorCommandRequest>
{
    public AttachAuthorCommandValidator()
    {
        RuleFor(x => x.Value.AuthorId).NotNull().NotEmpty();
        RuleFor(x => x.Value.DiskId).NotNull().NotEmpty();
    }
}