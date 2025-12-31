using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinance.Pluggy.Domain.SyncHistory;

namespace MyFinance.Pluggy.Infrastructure.Database.Configurations;

internal class SyncHistoryConfiguration : IEntityTypeConfiguration<SyncHistory>
{
    public void Configure(EntityTypeBuilder<SyncHistory> builder)
    {
        builder.ToTable("sync_histories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(x => x.ItemId)
            .HasColumnName("item_id")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Trigger)
            .HasColumnName("trigger")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.StartedAt)
            .HasColumnName("started_at")
            .IsRequired();

        builder.Property(x => x.CompletedAt)
            .HasColumnName("completed_at");

        builder.Property(x => x.TransactionsFetched)
            .HasColumnName("transactions_fetched")
            .IsRequired();

        builder.Property(x => x.TransactionsCreated)
            .HasColumnName("transactions_created")
            .IsRequired();

        builder.Property(x => x.TransactionsFailed)
            .HasColumnName("transactions_failed")
            .IsRequired();

        builder.Property(x => x.Success)
            .HasColumnName("success")
            .IsRequired();

        builder.Property(x => x.ErrorMessage)
            .HasColumnName("error_message")
            .HasMaxLength(1000);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Property(x => x.DeletedAt)
            .HasColumnName("deleted_at");

        builder.Property(x => x.IsDeleted)
            .HasColumnName("is_deleted")
            .IsRequired();

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.ItemId);
        builder.HasIndex(x => x.StartedAt);
    }
}
