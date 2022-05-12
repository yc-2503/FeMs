using IdentityService.Domain;
using IdentityService.Domain.Entitys;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure
{
    public class AuDbcontex:IdentityDbContext<User,Role,uint>
    {
        public AuDbcontex(DbContextOptions<AuDbcontex> options)
            :base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
            //modelBuilder.EnableSoftDeletionGlobalFilter();
        }
    }
}