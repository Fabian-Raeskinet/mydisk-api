using MediatR;
using MyDisk.Domain.Models;
using MyDisk.Infrastructure.Persistence;
using MyDisk.Services.Disks.DTOs;

namespace MyDisk.Services.Disks.Queries
{
    public class GetDiskByNameQuery : IRequest<DiskResponse>
    {
        public string? Name { get; set; }
    }

    public class GetDiskByNameHandler : IRequestHandler<GetDiskByNameQuery, DiskResponse>
    {
        public async Task<DiskResponse?> Handle(GetDiskByNameQuery request, CancellationToken cancellationToken)
        {
            var data = StaticContent.ContextData.Where(d => d.Name == request.Name).FirstOrDefault();
            if (data != null)
            {
                return new DiskResponse { 
                    Name = data.Name,
                    Author = data.Author,
                    ReleaseDate = data.ReleaseDate 
                };
            }
            return null;
        }
    }
}
