using FluentValidation;
using MediatorExtension;
using MyDisk.Contracts.Disks;

namespace Contracts.Validators.Disks;

public class GetDiskByNameQueryValidator : AbstractValidator<Request<GetDiskByNameQuery, DiskResponse>>
{
    public GetDiskByNameQueryValidator()
    {
        RuleFor(x => x.Value.Name).NotEmpty();
    }
}