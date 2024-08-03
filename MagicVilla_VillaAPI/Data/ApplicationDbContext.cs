using MagicVilla_VillaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Define DbSets for your entities
        public DbSet<Villa> Villas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // You can still configure entities here if needed, like defining table names, relationships, etc.
            ConfigureVillaEntity(modelBuilder);
        }

        private void ConfigureVillaEntity(ModelBuilder modelBuilder)
        {
            // Example of further entity configuration if needed, such as relationships or additional constraints
            modelBuilder.Entity<Villa>()
                .Property(v => v.Name)
                .HasMaxLength(100)
                .IsRequired();

            // Add more configurations as needed
        }
    }
}
