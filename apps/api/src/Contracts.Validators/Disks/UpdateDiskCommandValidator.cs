using FluentValidation;
using MyDisks.Services.Disks;

namespace MyDisks.Contracts.Validators.Disks;

public class UpdateDiskCommandValidator : AbstractValidator<UpdateDiskCommandRequest>
{
    public UpdateDiskCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
    }
}