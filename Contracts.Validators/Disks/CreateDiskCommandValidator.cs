using Contracts.Disks;
using FluentValidation;

namespace Contracts.Validators.Disks;

public class CreateDiskCommandValidator : AbstractValidator<CreateDiskCommand>
{
    public CreateDiskCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.ReleaseDate).NotEmpty();
    }
}