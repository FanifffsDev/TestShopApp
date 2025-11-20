using Microsoft.EntityFrameworkCore;

namespace TestShopApp.Common.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
}