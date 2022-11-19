using MediatR;
using MyDisk.Domain.Models;
using MyDisk.Infrastructure.Persistence;

namespace MyDisk.Services.Disks.Command
{
    public class CreateDiskCommand : IRequest<Guid>
    {
        public string Name { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string? Author { get; set; }
    }

    public class CreateDiskCommandHandler : IRequestHandler<CreateDiskCommand, Guid>
    {
        public async Task<Guid> Handle(CreateDiskCommand request, CancellationToken cancellationToken)
        {
            var entity = new Disk
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                ReleaseDate = request.ReleaseDate,
                Author = request.Author,
                CreatedDateTime = DateTime.Now
            };

            StaticContent.ContextData.Add(entity);

            return entity.Id;
        }
    }
}
