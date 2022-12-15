using FluentValidation;
using MediatR;
using MyDisk.Services.Disks.DTOs;

namespace MyDisk.Services.Disks.Requests;

public class GetDiskByNameRequest : IRequest<DiskResponse>
{
    public string? Name { get; init; }
}

public abstract class GetDiskByNameRequestValidator : AbstractValidator<GetDiskByNameRequest>
{
    protected GetDiskByNameRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().NotNull();
    }
}