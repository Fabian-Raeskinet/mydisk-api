using FluentValidation;
using MyDisk.Contracts.Disks;

namespace Contracts.Validators.Disks;

public class DeleteDiskCommandValidator : AbstractValidator<DeleteDiskCommand>
{
    public DeleteDiskCommandValidator()
    {
        RuleFor(x => x.Value).NotEmpty().NotNull();
        RuleFor(x => x.Property).NotEmpty().NotNull().IsInEnum();
    }
}