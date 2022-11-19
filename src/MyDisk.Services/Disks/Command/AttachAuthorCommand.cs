using AutoMapper;
using MediatR;
using MyDisk.Infrastructure.Persistence;
using MyDisk.Services.Disks.DTOs;

namespace MyDisk.Services.Disks.Command
{
    public class AttachAuthorCommand : IRequest<DiskResponse>
    {
        public Guid AuthorId { get; set; }
        public Guid DiskId { get; set; }
    }

    public class Handler : IRequestHandler<AttachAuthorCommand, DiskResponse>
    {
        IMapper _mapper;
        public Handler(IMapper mapper)
        {
            _mapper = mapper;
        }
        public async Task<DiskResponse>? Handle(AttachAuthorCommand request, CancellationToken cancellationToken)
        {
            var author = StaticContent.AuthorData.Where(a => a.Id == request.AuthorId).FirstOrDefault();
            var disk = StaticContent.DiskData.Where(d => d.Id == request.DiskId).FirstOrDefault();

            if (author == null && disk == null)
                return null;

            disk.Author = author;

            return _mapper.Map<DiskResponse>(disk);
        }
    }
}
