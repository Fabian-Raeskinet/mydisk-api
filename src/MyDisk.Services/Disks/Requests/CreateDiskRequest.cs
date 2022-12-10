using FluentValidation;
using MediatR;

namespace MyDisk.Services.Disks.Requests
{
    public class CreateDiskRequest : IRequest<Guid>
    {
        public string? Name { get; set; }
        public string? ReleaseDate { get; set; }
    }

    public class CreateDiskRequestValidator : AbstractValidator<CreateDiskRequest>
    {
        public CreateDiskRequestValidator() { 
            RuleFor(x => x.Name).NotEmpty().NotNull();
            RuleFor(x => x.ReleaseDate).NotEmpty().NotNull();
        }
    }
}