using MyDisk.Domain.Models;
using MyDisk.Infrastructure.Interfaces.IRepositories;
using MyDisk.Infrastructure.Persistence;

namespace MyDisk.Infrastructure.Repositories;

public class AuthorRepository : IAuthorRepository
{
    public Author? GetAuthorByFilter(Func<Author, bool> predicate) => StaticContent.AuthorData.FirstOrDefault(predicate);
}