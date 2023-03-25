using AutoMapper;
using MediatorExtension.Disks;
using MediatR;
using MyDisk.Domain;
using MyDisk.Domain.Exceptions;

namespace MyDisk.Services.Disks;

public class AttachAuthorCommandHandler : IRequestHandler<AttachAuthorCommandRequest, Unit>
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

    public async Task<Unit> Handle(AttachAuthorCommandRequest request, CancellationToken cancellationToken)
    {
        var author = await AuthorRepository.GetAuthorByFilterAsync(x => x.Id == request.AuthorId);
        var disk = await DiskRepository.GetDiskByFilterAsync(x => x.Id == request.DiskId);

        if (author == null || disk == null)
            throw new ObjectNotFoundException("no matches found");

        disk.Author = author;

        return Unit.Value;
    }
}