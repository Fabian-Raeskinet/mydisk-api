using Contracts.Disks;
using FluentValidation;

namespace Contracts.Validators.Disks;

public class DeleteDiskCommandValidator : AbstractValidator<DeleteDiskCommand>
{
    public DeleteDiskCommandValidator() { 
        RuleFor(x => x.Value).NotEmpty().NotNull();
    }
}