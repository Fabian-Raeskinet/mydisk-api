using AutoMapper;
using MediatR;
using MyDisk.Domain.Exceptions;
using MyDisk.Domain.Interfaces.IRepositories;
using MyDisk.Services.Disks.DTOs;
using MyDisk.Services.Disks.Requests;

namespace MyDisk.Services.Disks.Commands;

public class AttachAuthorCommandHandler : IRequestHandler<AttachAuthorRequest, DiskResponse>
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

    public async Task<DiskResponse> Handle(AttachAuthorRequest request, CancellationToken cancellationToken)
    {
        var author = await AuthorRepository.GetAuthorByFilterAsync(x => x.Id == request.AuthorId);
        var disk = await DiskRepository.GetDiskByFilterAsync(x => x.Id == request.DiskId);

        if (author == null || disk == null)
            throw new ObjectNotFoundException("no matches found");

        disk.Author = author;

        return Mapper.Map<DiskResponse>(disk);
    }
}