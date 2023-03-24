using AutoMapper;
using MediatorExtension;
using MyDisk.Contracts.Disks;
using MyDisk.Domain;
using MyDisk.Domain.Exceptions;

namespace MyDisk.Services.Disks;

public class AttachAuthorCommandHandler : RequestHandler<AttachAuthorCommand, DiskResponse>
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

    public override async Task<DiskResponse> Handle(Request<AttachAuthorCommand, DiskResponse> request,
        CancellationToken cancellationToken)
    {
        var author = await AuthorRepository.GetAuthorByFilterAsync(x => x.Id == request.Value.AuthorId);
        var disk = await DiskRepository.GetDiskByFilterAsync(x => x.Id == request.Value.DiskId);

        if (author == null || disk == null)
            throw new ObjectNotFoundException("no matches found");

        disk.Author = author;

        return Mapper.Map<DiskResponse>(disk);
    }
}