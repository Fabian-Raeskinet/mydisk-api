using FluentValidation;
using MediatR;
using MyDisk.Services.Disks.DTOs;

namespace MyDisk.Services.Disks.Requests
{
    public class GetDiskByNameRequest : IRequest<DiskResponse>
    {
        public string? Name { get; set; }
    }

    public class GetDiskByNameRequestValidator : AbstractValidator<GetDiskByNameRequest>
    {
        public GetDiskByNameRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().NotNull();
        }
    }
}

