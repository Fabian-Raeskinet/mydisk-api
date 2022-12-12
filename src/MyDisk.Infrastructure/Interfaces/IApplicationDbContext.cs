using MyDisk.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace MyDisk.Services.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Disk> Disks { get; }
    }
}
