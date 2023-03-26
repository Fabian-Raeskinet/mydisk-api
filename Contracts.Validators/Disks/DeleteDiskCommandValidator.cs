using FluentValidation;
using MyDisk.Services.Disks;

namespace Contracts.Validators.Disks;

public class DeleteDiskCommandValidator : AbstractValidator<DeleteDiskCommandRequest>
{
    public DeleteDiskCommandValidator()
    {
        RuleFor(x => x.Value).NotEmpty().NotNull();
        RuleFor(x => x.Property).NotEmpty().NotNull().IsInEnum();
    }
}