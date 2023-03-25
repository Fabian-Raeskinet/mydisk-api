using FluentValidation;
using MediatorExtension;
using MediatR;
using MyDisk.Contracts.Disks;

namespace Contracts.Validators.Disks;

public class UpdateDiskCommandValidator : AbstractValidator<Request<UpdateDiskCommand, Unit>>
{
    public UpdateDiskCommandValidator()
    {
        RuleFor(x => x.Value.Id).NotEmpty().NotNull();
    }
}