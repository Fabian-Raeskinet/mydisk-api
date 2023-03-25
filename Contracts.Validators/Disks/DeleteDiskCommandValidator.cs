using FluentValidation;
using MediatorExtension;
using MediatR;
using MyDisk.Contracts.Disks;

namespace Contracts.Validators.Disks;

public class DeleteDiskCommandValidator : AbstractValidator<Request<DeleteDiskCommand, Unit>>
{
    public DeleteDiskCommandValidator()
    {
        RuleFor(x => x.Value.Value).NotEmpty().NotNull();
        RuleFor(x => x.Value.Property).NotEmpty().NotNull().IsInEnum();
    }
}