using FluentValidation;
using MyDisks.Services.Disks;

namespace MyDisks.Contracts.Validators.Disks;

public class GetDiskByNameQueryValidator : AbstractValidator<GetDiskByNameQueryRequest>
{
    public GetDiskByNameQueryValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}