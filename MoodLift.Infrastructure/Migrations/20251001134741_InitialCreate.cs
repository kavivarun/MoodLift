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
                    MoodWord = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    PrimaryEmotion = table.Column<int>(type: "INTEGER", nullable: false),
                    Symptoms = table.Column<int>(type: "INTEGER", nullable: false),
                    SleepHours = table.Column<int>(type: "INTEGER", nullable: false),
                    EnergyLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    CaffeineDrinks = table.Column<int>(type: "INTEGER", nullable: false),
                    Movement = table.Column<int>(type: "INTEGER", nullable: false),
                    StressScore = table.Column<int>(type: "INTEGER", nullable: false),
                    StressCause = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    CopingStrategies = table.Column<int>(type: "INTEGER", nullable: false),
                    NextActions = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    PositiveThing = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true),
                    GoogleUserId = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoodEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GratitudeItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MoodEntryId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Text = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GratitudeItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GratitudeItems_MoodEntries_MoodEntryId",
                        column: x => x.MoodEntryId,
                        principalTable: "MoodEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GratitudeItems_MoodEntryId_DisplayOrder",
                table: "GratitudeItems",
                columns: new[] { "MoodEntryId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_MoodEntries_GoogleUserId_CreatedAtUtc",
                table: "MoodEntries",
                columns: new[] { "GoogleUserId", "CreatedAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GratitudeItems");

            migrationBuilder.DropTable(
                name: "MoodEntries");
        }
    }
}
