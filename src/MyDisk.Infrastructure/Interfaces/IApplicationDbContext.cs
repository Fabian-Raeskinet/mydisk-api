using Microsoft.EntityFrameworkCore;
using MyDisk.Domain.Models;

namespace MyDisk.Infrastructure.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Disk> Disks { get; }
}