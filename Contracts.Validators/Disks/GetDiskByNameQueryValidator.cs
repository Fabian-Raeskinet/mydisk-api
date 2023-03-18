using Contracts.Disks;
using FluentValidation;

namespace Contracts.Validators.Disks;

public class GetDiskByNameQueryValidator : AbstractValidator<GetDiskByNameQuery>
{
    public GetDiskByNameQueryValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}