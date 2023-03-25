using FluentValidation;
using MediatorExtension;
using MediatR;
using MyDisk.Contracts.Disks;

namespace Contracts.Validators.Disks;

public class AttachAuthorCommandValidator : AbstractValidator<Request<AttachAuthorCommand, Unit>>
{
    public AttachAuthorCommandValidator()
    {
        RuleFor(x => x.Value.AuthorId).NotNull().NotEmpty();
        RuleFor(x => x.Value.DiskId).NotNull().NotEmpty();
    }
}