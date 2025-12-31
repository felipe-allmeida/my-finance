using Microsoft.EntityFrameworkCore;
using MyFinance.Pluggy.Domain.Connections;
using MyFinance.Pluggy.Domain.SyncHistory;

namespace MyFinance.Pluggy.Infrastructure.Database;

public sealed class PluggyContext(DbContextOptions<PluggyContext> options) : DbContext(options)
{
    public DbSet<SyncHistory> SyncHistories => Set<SyncHistory>();
    public DbSet<UserConnection> UserConnections => Set<UserConnection>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("pluggy");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PluggyContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<Enum>().HaveConversion<string>();
    }
}
