using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFinance.Pluggy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "pluggy");

            migrationBuilder.CreateTable(
                name: "sync_histories",
                schema: "pluggy",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    trigger = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    transactions_fetched = table.Column<int>(type: "integer", nullable: false),
                    transactions_created = table.Column<int>(type: "integer", nullable: false),
                    transactions_failed = table.Column<int>(type: "integer", nullable: false),
                    success = table.Column<bool>(type: "boolean", nullable: false),
                    error_message = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sync_histories", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_sync_histories_item_id",
                schema: "pluggy",
                table: "sync_histories",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "ix_sync_histories_started_at",
                schema: "pluggy",
                table: "sync_histories",
                column: "started_at");

            migrationBuilder.CreateIndex(
                name: "ix_sync_histories_user_id",
                schema: "pluggy",
                table: "sync_histories",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sync_histories",
                schema: "pluggy");
        }
    }
}
