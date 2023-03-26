using FluentValidation;
using MyDisk.Services.Disks;

namespace Contracts.Validators.Disks;

public class UpdateDiskCommandValidator : AbstractValidator<UpdateDiskCommandRequest>
{
    public UpdateDiskCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
    }
}