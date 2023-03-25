using FluentValidation;
using MediatorExtension.Disks;

namespace Contracts.Validators.Disks;

public class CreateDiskCommandValidator : AbstractValidator<CreateDiskCommandRequest>
{
    public CreateDiskCommandValidator()
    {
        RuleFor(x => x.Value.Name).NotEmpty();
        RuleFor(x => x.Value.ReleaseDate).NotEmpty();
    }
}