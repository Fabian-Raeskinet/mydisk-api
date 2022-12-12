using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using MyDisk.Infrastructure.Persistence.Identity;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.Extensions.Options;
using MyDisk.Services.Common.Interfaces;
using MyDisk.Domain.Models;

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
