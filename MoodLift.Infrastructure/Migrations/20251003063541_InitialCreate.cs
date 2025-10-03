using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoodLift.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MoodEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MoodScore = table.Column<int>(type: "INTEGER", nullable: false),
                    PrimaryEmotion = table.Column<int>(type: "INTEGER", nullable: false),
                    Symptoms = table.Column<int>(type: "INTEGER", nullable: false),
                    SleepHours = table.Column<int>(type: "INTEGER", nullable: false),
                    EnergyLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    CaffeineDrinks = table.Column<int>(type: "INTEGER", nullable: false),
                    StressScore = table.Column<int>(type: "INTEGER", nullable: false),
                    CopingStrategies = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true),
                    GoogleUserId = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoodEntries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MoodEntries_GoogleUserId_CreatedAtUtc",
                table: "MoodEntries",
                columns: new[] { "GoogleUserId", "CreatedAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MoodEntries");
        }
    }
}
