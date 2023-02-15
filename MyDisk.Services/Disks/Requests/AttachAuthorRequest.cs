using FluentValidation;
using MediatR;
using MyDisk.Services.Disks.DTOs;

namespace MyDisk.Services.Disks.Requests;

public class AttachAuthorRequest : IRequest<DiskResponse>
{
    public Guid AuthorId { get; set; }
    public Guid DiskId { get; set; }
}

public class AttachAuthorRequestValidator : AbstractValidator<AttachAuthorRequest>
{
    public AttachAuthorRequestValidator()
    {
        RuleFor(x => x.AuthorId).NotNull().NotEmpty();
        RuleFor(x => x.DiskId).NotNull().NotEmpty();
    }
}