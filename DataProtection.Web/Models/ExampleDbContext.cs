using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DataProtection.Web.Models
{
    public partial class ExampleDbContext : DbContext
    {
        public ExampleDbContext()
        {
        }

        public ExampleDbContext(DbContextOptions<ExampleDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Product> Product { get; set; }
          
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Color)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 4)");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
