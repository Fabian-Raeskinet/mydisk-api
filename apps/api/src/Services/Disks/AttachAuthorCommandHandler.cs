using AutoMapper;
using MediatR;
using MyDisks.Domain;
using MyDisks.Domain.Authors;
using MyDisks.Domain.Disks;
using MyDisks.Domain.Exceptions;

namespace MyDisks.Services.Disks;

public class AttachAuthorCommandHandler : ICommandHandler<AttachAuthorCommandRequest>
{
    public AttachAuthorCommandHandler(IMapper mapper, IUnitOfWork unitOfWork)
    {
        Mapper = mapper;
        UnitOfWork = unitOfWork;
    }

    public IMapper Mapper { get; }
    private IUnitOfWork UnitOfWork { get; }

    public async Task Handle(AttachAuthorCommandRequest request, CancellationToken cancellationToken)
    {
        
        var author = await UnitOfWork.AuthorRepository.GetAuthorByFilterAsync(x => x.Id == request.AuthorId);
        var disk = await UnitOfWork.DiskRepository.GetDiskByFilterAsync(x => x.Id == request.DiskId);

        if (author is null || disk is null)
            throw new ObjectNotFoundException("no matches found");

        disk.AttachAuthor(author);
        await UnitOfWork.CommitAsync();
    }
}