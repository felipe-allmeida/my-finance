using Microsoft.EntityFrameworkCore;

namespace MyFinance.Ledger.Infrastructure.Database
{
    internal class LedgerContext(DbContextOptions<LedgerContext> options) : DbContext(options)
    {
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<Enum>().HaveConversion<string>();
        }
    }
}
