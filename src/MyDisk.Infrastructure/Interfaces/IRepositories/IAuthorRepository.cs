using MyDisk.Domain.Models;

namespace MyDisk.Infrastructure.Interfaces.IRepositories;

public interface IAuthorRepository
{
    public Author? GetAuthorByFilter(Func<Author, bool> predicate);
}