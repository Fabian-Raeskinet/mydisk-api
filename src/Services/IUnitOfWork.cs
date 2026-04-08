using MyDisks.Domain.Authors;
using MyDisks.Domain.Disks;

namespace MyDisks.Services;

public interface IUnitOfWork
{
    IAuthorRepository AuthorRepository { get; }
    IDiskRepository DiskRepository { get; }
    Task<int> CommitAsync();
}