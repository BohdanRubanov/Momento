using Microsoft.EntityFrameworkCore;
using Momento.Models;

namespace Momento.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Registration> Users { get; set; } = null!;
        public DbSet<Publication> Publications { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка отношения один-ко-многим: один пользователь — много публикаций
            modelBuilder.Entity<Publication>()
                .HasOne(p => p.User)
                .WithMany(u => u.Publications)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Каскадное удаление: если удалить пользователя, удалятся его публикации
        }
    }
}