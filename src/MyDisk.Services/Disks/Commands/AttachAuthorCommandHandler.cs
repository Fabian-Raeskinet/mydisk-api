using AutoMapper;
using MediatR;
using MyDisk.Domain.Interfaces.IRepositories;
using MyDisk.Services.Common.Exceptions;
using MyDisk.Services.Disks.DTOs;
using MyDisk.Services.Disks.Requests;

namespace MyDisk.Services.Disks.Commands;

public class AttachAuthorCommandHandler : IRequestHandler<AttachAuthorRequest, DiskResponse>
{
    private readonly IMapper _mapper;
    private readonly IAuthorRepository _authorRepository;
    private readonly IDiskRepository _diskRepository;
    public AttachAuthorCommandHandler(IMapper mapper, IAuthorRepository repository, IAuthorRepository authorRepository, IDiskRepository diskRepository)
    {
        _mapper = mapper;
        _authorRepository = authorRepository;
        _diskRepository = diskRepository;
    }

    public async Task<DiskResponse> Handle(AttachAuthorRequest request, CancellationToken cancellationToken)
    {
        var author = await _authorRepository.GetAuthorByFilterAsync(x => x.Id == request.AuthorId);
        var disk = await  _diskRepository.GetDiskByFilterAsync(x => x.Id == request.DiskId);

        if (author == null || disk == null)
            throw new EntityNotFoundException("no matches found");

        disk.Author = author;

        return _mapper.Map<DiskResponse>(disk);
    }
}