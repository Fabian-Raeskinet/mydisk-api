using FluentValidation;
using MediatR;
using MyDisk.Services.Common.Enums;

namespace MyDisk.Services.Disks.Requests;

public class DeleteDiskRequest : IRequest<Unit>
{
    public DeleteDiskByPropertyEnum? Property { get; set; }
    public string? Value { get; set; }
}

public class DeleteDiskRequestValidator : AbstractValidator<DeleteDiskRequest>
{
    public DeleteDiskRequestValidator() { 
        RuleFor(x => x.Value).NotEmpty().NotNull();
    }
}