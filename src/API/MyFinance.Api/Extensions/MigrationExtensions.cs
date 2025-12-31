using Microsoft.EntityFrameworkCore;
using MyFinance.Ledger.Infrastructure.Database;
using MyFinance.Pluggy.Infrastructure.Database;

namespace MyFinance.Api.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        ApplyMigration<LedgerContext>(scope);
        ApplyMigration<PluggyContext>(scope);
    }

    private static void ApplyMigration<TDbContext>(IServiceScope scope) where TDbContext : DbContext
    {
        using var context = scope.ServiceProvider.GetRequiredService<TDbContext>();

        context.Database.Migrate();
    }
}
