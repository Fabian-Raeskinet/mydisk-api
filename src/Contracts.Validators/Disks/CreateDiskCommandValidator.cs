using FluentValidation;
using MyDisks.Services.Disks;

namespace MyDisks.Contracts.Validators.Disks;

public class CreateDiskCommandValidator : AbstractValidator<CreateDiskCommandRequest>
{
    public CreateDiskCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.ReleaseDate).NotEmpty();
    }
}