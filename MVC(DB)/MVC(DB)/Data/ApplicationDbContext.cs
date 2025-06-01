using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MVC_DB_.Models;

namespace MVC_DB_.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<account> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Campaign>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.TargetAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.CollectedAmount).HasColumnType("decimal(18,2)");
                entity.HasOne(d => d.Owner)
                    .WithMany()
                    .HasForeignKey(d => d.OwnerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<account>(entity =>
            {
                entity.ToTable("accountInformation");
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).HasColumnName("id");
                entity.Property(e => e.userName).HasColumnName("userName").IsRequired().HasMaxLength(50);
                entity.Property(e => e.passwd).HasColumnName("passwd").IsRequired().HasMaxLength(100);
                entity.Property(e => e.name).HasColumnName("name").IsRequired().HasMaxLength(100);
            });
        }
    }
} 