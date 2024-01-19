using FluentValidation;
using MyDisks.Services.Disks;

namespace MyDisks.Contracts.Validators.Disks;

public class DeleteDiskCommandValidator : AbstractValidator<DeleteDiskCommandRequest>
{
    public DeleteDiskCommandValidator()
    {
        RuleFor(x => x.DiskId).NotEmpty().NotNull();
    }
}