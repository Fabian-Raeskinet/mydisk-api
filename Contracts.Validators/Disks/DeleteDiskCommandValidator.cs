using FluentValidation;
using MediatorExtension.Disks;

namespace Contracts.Validators.Disks;

public class DeleteDiskCommandValidator : AbstractValidator<DeleteDiskCommandRequest>
{
    public DeleteDiskCommandValidator()
    {
        RuleFor(x => x.Value.Value).NotEmpty().NotNull();
        RuleFor(x => x.Value.Property).NotEmpty().NotNull().IsInEnum();
    }
}