using DomainDrivenDesign.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DomainDrivenDesign.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Category> Categorys { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //table category
            modelBuilder.Entity<Student>(entity =>
             {
                 entity.HasKey(e => e.StudentId);
                 entity.Property(e => e.StudentFullName).IsRequired().HasMaxLength(100);
                 entity.Property(e => e.StudentClass).IsRequired();
                 entity.Property(e => e.StudentAge).IsRequired();
                 entity.Property(e => e.StudentAddress).IsRequired();
                 entity.Property(e => e.CategoryId).IsRequired();

                 entity.HasOne(e => e.Category)
                       .WithMany(c => c.Students)
                       .HasForeignKey(e => e.CategoryId)
                       .OnDelete(DeleteBehavior.Cascade);
             });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CategoryId);
                entity.Property(e => e.CategoryName).IsRequired().HasMaxLength(100);
            });

            //table user
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.UserName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.UserEmail).IsRequired();
                entity.Property(e => e.UserPassword).IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}