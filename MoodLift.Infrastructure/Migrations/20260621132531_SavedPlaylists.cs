using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoodLift.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SavedPlaylists : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SavedPlaylists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    SearchQuery = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Reason = table.Column<string>(type: "TEXT", maxLength: 600, nullable: false),
                    WorkplaceUse = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    TrackIdsJson = table.Column<string>(type: "TEXT", nullable: false),
                    GoogleUserId = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedPlaylists", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SavedPlaylists_GoogleUserId_CreatedAtUtc",
                table: "SavedPlaylists",
                columns: new[] { "GoogleUserId", "CreatedAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SavedPlaylists");
        }
    }
}
