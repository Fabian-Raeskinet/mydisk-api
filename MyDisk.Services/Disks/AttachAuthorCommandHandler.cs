using AutoMapper;
using Contracts.Disks;
using MediatR;
using MyDisk.Domain;
using MyDisk.Domain.Exceptions;

namespace MyDisk.Services.Disks;

public class AttachAuthorCommandHandler : IRequestHandler<AttachAuthorCommand, DiskResponse>
{
    public AttachAuthorCommandHandler(IMapper mapper, IAuthorRepository repository, IAuthorRepository authorRepository,
        IDiskRepository diskRepository)
    {
        Mapper = mapper;
        AuthorRepository = authorRepository;
        DiskRepository = diskRepository;
    }

    public IMapper Mapper { get; }
    public IAuthorRepository AuthorRepository { get; }
    public IDiskRepository DiskRepository { get; }

    public async Task<DiskResponse> Handle(AttachAuthorCommand request, CancellationToken cancellationToken)
    {
        var author = await AuthorRepository.GetAuthorByFilterAsync(x => x.Id == request.AuthorId);
        var disk = await DiskRepository.GetDiskByFilterAsync(x => x.Id == request.DiskId);

        if (author == null || disk == null)
            throw new ObjectNotFoundException("no matches found");

        disk.Author = author;

        return Mapper.Map<DiskResponse>(disk);
    }
}