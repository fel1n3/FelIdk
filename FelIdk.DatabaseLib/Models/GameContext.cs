using Microsoft.EntityFrameworkCore;

namespace FelIdk.DatabaseLib.Models;

public class GameContext : DbContext
{
    public DbSet<Ability> Abilities { get; set; }

    public GameContext()
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=data.db");
}