using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GameForge.API.Migrations
{
    /// <inheritdoc />
    public partial class AddItemCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Rarity = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<int>(type: "integer", nullable: false),
                    MaxStack = table.Column<int>(type: "integer", nullable: false),
                    AttackBonus = table.Column<int>(type: "integer", nullable: false),
                    DefenseBonus = table.Column<int>(type: "integer", nullable: false),
                    HealthRestore = table.Column<int>(type: "integer", nullable: false),
                    ManaRestore = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "Id", "AttackBonus", "CreatedAt", "DefenseBonus", "Description", "HealthRestore", "ManaRestore", "MaxStack", "Name", "Rarity", "Type", "Value" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), 15, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, "A sturdy standard-issue iron longsword.", 0, 0, 1, "Iron Longsword", 0, 0, 50 },
                    { new Guid("22222222-2222-2222-2222-222222222222"), 12, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, "A wooden wand that channels basic arcane power.", 0, 0, 1, "Apprentice Wand", 0, 0, 45 },
                    { new Guid("33333333-3333-3333-3333-333333333333"), 0, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 25, "Heavy armor forged to withstand crushing blows.", 0, 0, 1, "Steel Plate Armor", 1, 1, 120 },
                    { new Guid("44444444-4444-4444-4444-444444444444"), 0, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, "Instantly restores 50 Health points.", 50, 0, 99, "Lesser Health Potion", 0, 2, 15 },
                    { new Guid("55555555-5555-5555-5555-555555555555"), 0, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, "Instantly restores 30 Mana points.", 0, 30, 99, "Lesser Mana Potion", 0, 2, 15 },
                    { new Guid("66666666-6666-6666-6666-666666666666"), 150, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, "A legendary greatsword imbued with primordial flame.", 0, 0, 1, "Dragon Slayer Blade", 4, 0, 2500 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Items");
        }
    }
}
