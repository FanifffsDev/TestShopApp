using Microsoft.EntityFrameworkCore;
using TestShopApp.Domain.Base;

namespace TestShopApp.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Group> Groups { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
        .HasOne(u => u.Group)
        .WithMany(g => g.Students)
        .HasForeignKey(u => u.GroupNumber)
        .OnDelete(DeleteBehavior.SetNull); // При удалении группы GroupNumber = null

        modelBuilder.Entity<User>()
            .HasIndex(u => u.GroupNumber);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.HeadmanOf);

        // Связь User -> Group (через HeadmanOf - староста группы)
        modelBuilder.Entity<User>()
            .HasOne<Group>()
            .WithOne()
            .HasForeignKey<User>(u => u.HeadmanOf)
            .HasPrincipalKey<Group>(g => g.Number)
            .OnDelete(DeleteBehavior.SetNull);

        base.OnModelCreating(modelBuilder);
    }
}