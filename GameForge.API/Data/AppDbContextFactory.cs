using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GameForge.API.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // Use the connection string directly for design-time migrations
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=gameforge_db;Username=postgres;Password=Abdullah@302");

        return new AppDbContext(optionsBuilder.Options);
    }
}