using FluentValidation;
using MediatorExtension;
using MediatR;
using MyDisk.Contracts.Disks;

namespace Contracts.Validators.Disks;

public class CreateDiskCommandValidator : AbstractValidator<Request<CreateDiskCommand, Unit>>
{
    public CreateDiskCommandValidator()
    {
        RuleFor(x => x.Value.Name).NotEmpty();
        RuleFor(x => x.Value.ReleaseDate).NotEmpty();
    }
}