using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GameForge.API.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ItemId1",
                table: "InventoryItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Quests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    RequiredLevel = table.Column<int>(type: "integer", nullable: false),
                    ExperienceReward = table.Column<int>(type: "integer", nullable: false),
                    GuaranteedItemRewardId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quests_Items_GuaranteedItemRewardId",
                        column: x => x.GuaranteedItemRewardId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "AttackBonus", "CreatedAt", "Description", "Name", "Value" },
                values: new object[] { 5, new DateTime(2026, 8, 20, 19, 9, 55, 937, DateTimeKind.Utc).AddTicks(4987), "A weathered iron blade.", "Rusty Sword", 0 });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "AttackBonus", "CreatedAt", "DefenseBonus", "Description", "Name", "Rarity", "Type", "Value" },
                values: new object[] { 0, new DateTime(2026, 8, 20, 19, 9, 55, 938, DateTimeKind.Utc).AddTicks(31), 12, "Sturdy steel breastplate.", "Iron Plate Armor", 1, 1, 0 });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "AttackBonus", "CreatedAt", "Description", "Name", "Rarity", "Type", "Value" },
                values: new object[] { 150, new DateTime(2026, 8, 20, 19, 9, 55, 938, DateTimeKind.Utc).AddTicks(68), "Forged in ancient dragon fire.", "Dragon Slayer Blade", 4, 0, 0 });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "Description", "Value" },
                values: new object[] { new DateTime(2026, 8, 20, 19, 9, 55, 938, DateTimeKind.Utc).AddTicks(73), "Restores 50 hit points.", 0 });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "Description", "ManaRestore", "Name", "Value" },
                values: new object[] { new DateTime(2026, 8, 20, 19, 9, 55, 938, DateTimeKind.Utc).AddTicks(538), "Restores 35 mana points.", 35, "Mana Flask", 0 });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "AttackBonus", "CreatedAt", "DefenseBonus", "Description", "Name", "Rarity", "Type", "Value" },
                values: new object[] { 0, new DateTime(2026, 8, 20, 19, 9, 55, 938, DateTimeKind.Utc).AddTicks(963), 3, "Light and agile footwear.", "Leather Boots", 0, 1, 0 });

            migrationBuilder.InsertData(
                table: "Quests",
                columns: new[] { "Id", "Description", "ExperienceReward", "GuaranteedItemRewardId", "RequiredLevel", "Title" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), "Clear the cellar of vermin for the local tavernkeeper.", 50, new Guid("44444444-4444-4444-4444-444444444444"), 1, "Rats in the Cellar" },
                    { new Guid("10000000-0000-0000-0000-000000000002"), "Infiltrate and dismantle the vanguard camp near the forest edge.", 150, new Guid("22222222-2222-2222-2222-222222222222"), 2, "The Goblin Outpost" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Players_Email",
                table: "Players",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_Username",
                table: "Players",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_ItemId1",
                table: "InventoryItems",
                column: "ItemId1");

            migrationBuilder.CreateIndex(
                name: "IX_Quests_GuaranteedItemRewardId",
                table: "Quests",
                column: "GuaranteedItemRewardId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_Items_ItemId1",
                table: "InventoryItems",
                column: "ItemId1",
                principalTable: "Items",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_Items_ItemId1",
                table: "InventoryItems");

            migrationBuilder.DropTable(
                name: "Quests");

            migrationBuilder.DropIndex(
                name: "IX_Players_Email",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_Username",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_InventoryItems_ItemId1",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "ItemId1",
                table: "InventoryItems");

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "AttackBonus", "CreatedAt", "Description", "Name", "Value" },
                values: new object[] { 15, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A sturdy standard-issue iron longsword.", "Iron Longsword", 50 });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "AttackBonus", "CreatedAt", "DefenseBonus", "Description", "Name", "Rarity", "Type", "Value" },
                values: new object[] { 12, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, "A wooden wand that channels basic arcane power.", "Apprentice Wand", 0, 0, 45 });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "AttackBonus", "CreatedAt", "Description", "Name", "Rarity", "Type", "Value" },
                values: new object[] { 0, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Heavy armor forged to withstand crushing blows.", "Steel Plate Armor", 1, 1, 120 });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "Description", "Value" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Instantly restores 50 Health points.", 15 });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "Description", "ManaRestore", "Name", "Value" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Instantly restores 30 Mana points.", 30, "Lesser Mana Potion", 15 });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "AttackBonus", "CreatedAt", "DefenseBonus", "Description", "Name", "Rarity", "Type", "Value" },
                values: new object[] { 150, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, "A legendary greatsword imbued with primordial flame.", "Dragon Slayer Blade", 4, 0, 2500 });
        }
    }
}
