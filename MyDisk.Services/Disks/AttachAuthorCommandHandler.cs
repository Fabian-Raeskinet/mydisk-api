using AutoMapper;
using MediatorExtension.Disks;
using MediatR;
using MyDisk.Domain;
using MyDisk.Domain.Exceptions;

namespace MyDisk.Services.Disks;

public class AttachAuthorCommandHandler : MediatorExtension.RequestHandler<AttachAuthorCommandRequest, Unit>
{
    public AttachAuthorCommandHandler(IMapper mapper, IAuthorRepository authorRepository,
        IDiskRepository diskRepository)
    {
        Mapper = mapper;
        AuthorRepository = authorRepository;
        DiskRepository = diskRepository;
    }

    public IMapper Mapper { get; }
    public IAuthorRepository AuthorRepository { get; }
    public IDiskRepository DiskRepository { get; }

    public override async Task<Unit> Handle(AttachAuthorCommandRequest request, CancellationToken cancellationToken)
    {
        var author = await AuthorRepository.GetAuthorByFilterAsync(x => x.Id == request.Value.AuthorId);
        var disk = await DiskRepository.GetDiskByFilterAsync(x => x.Id == request.Value.DiskId);

        if (author == null || disk == null)
            throw new ObjectNotFoundException("no matches found");

        disk.Author = author;

        return Unit.Value;
    }
}