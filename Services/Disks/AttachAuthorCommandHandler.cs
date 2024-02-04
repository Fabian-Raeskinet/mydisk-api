using AutoMapper;
using MediatR;
using MyDisks.Domain;
using MyDisks.Domain.Exceptions;

namespace MyDisks.Services.Disks;

public class AttachAuthorCommandHandler : ICommandHandler<AttachAuthorCommandRequest>
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

    public async Task Handle(AttachAuthorCommandRequest request, CancellationToken cancellationToken)
    {
        var author = await AuthorRepository.GetAuthorByFilterAsync(x => x.Id == request.AuthorId);
        var disk = await DiskRepository.GetDiskByFilterAsync(x => x.Id == request.DiskId);

        if (author == null || disk == null)
            throw new ObjectNotFoundException("no matches found");

        disk.Author = author;
    }
}