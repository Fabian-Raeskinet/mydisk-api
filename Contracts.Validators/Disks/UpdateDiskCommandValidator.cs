using FluentValidation;
using MyDisk.Contracts.Disks;

namespace Contracts.Validators.Disks;

public class UpdateDiskCommandValidator : AbstractValidator<UpdateDiskCommand>
{
    public UpdateDiskCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
    }
}