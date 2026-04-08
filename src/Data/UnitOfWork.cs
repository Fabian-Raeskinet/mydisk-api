using MyDisks.Domain.Authors;
using MyDisks.Domain.Disks;
using MyDisks.Services;

namespace MyDisks.Data;

public class UnitOfWork : IUnitOfWork
{
    public UnitOfWork(IApplicationDbContext context,
        IAuthorRepository authorRepository,
        IDiskRepository diskRepository)
    {
        Context = context;
        AuthorRepository = authorRepository;
        DiskRepository = diskRepository;
    }

    public IAuthorRepository AuthorRepository { get; }
    public IDiskRepository DiskRepository { get; }
    public IApplicationDbContext Context { get; }


    public async Task<int> CommitAsync()
    {
        return await Context.SaveChangesAsync();
    }
}