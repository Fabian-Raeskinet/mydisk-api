using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MyDisk.Infrastructure.Persistence.Interfaces;
using MyDisk.Domain.Models;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using MyDisk.Infrastructure.Persistence.Identity;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.Extensions.Options;

namespace MyDisk.Infrastructure.Persistence
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>, IApplicationDbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            IOptions<OperationalStoreOptions> operationalStoreOptions)
            : base(options, operationalStoreOptions)
        { }

        public DbSet<Disk> Disks => Set<Disk>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(builder);
        }

    }
}
