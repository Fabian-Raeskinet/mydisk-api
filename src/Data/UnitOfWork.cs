using Microsoft.EntityFrameworkCore.Storage;
using MyDisks.Domain.Authors;
using MyDisks.Domain.Disks;
using MyDisks.Services;

namespace MyDisks.Data;

public class UnitOfWork : IUnitOfWork
{
    private IDbContextTransaction? _currentTransaction;

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
    
    public async Task BeginTransactionAsync()
    {
        if (_currentTransaction == null)
        {
            _currentTransaction = await Context.BeginTransactionAsync();
        }
    }
    
    public async Task RollbackAsync()
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.RollbackAsync();
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }
    
    public async Task<int> CommitAsync()
    {
        try
        {
            var result = await Context.SaveChangesAsync();

            if (_currentTransaction != null)
            {
                await _currentTransaction.CommitAsync();
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }

            return result;
        }
        catch
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync();
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }

            throw;
        }
    }
}