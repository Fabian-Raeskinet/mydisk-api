using FluentValidation;
using MyDisk.Services.Disks;

namespace Contracts.Validators.Disks;

public class CreateDiskCommandValidator : AbstractValidator<CreateDiskCommandRequest>
{
    public CreateDiskCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.ReleaseDate).NotEmpty();
    }
}