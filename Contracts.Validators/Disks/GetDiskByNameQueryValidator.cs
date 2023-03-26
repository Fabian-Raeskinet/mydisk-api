using FluentValidation;
using MyDisk.Services.Disks;

namespace Contracts.Validators.Disks;

public class GetDiskByNameQueryValidator : AbstractValidator<GetDiskByNameQueryRequest>
{
    public GetDiskByNameQueryValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}