using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GameForge.API.Migrations
{
    /// <inheritdoc />
    public partial class AddMonsterSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Monsters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    MaxHealth = table.Column<int>(type: "integer", nullable: false),
                    AttackPower = table.Column<int>(type: "integer", nullable: false),
                    Defense = table.Column<int>(type: "integer", nullable: false),
                    ExperienceReward = table.Column<int>(type: "integer", nullable: false),
                    LootItemId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monsters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Monsters_Items_LootItemId",
                        column: x => x.LootItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2026, 8, 20, 19, 26, 31, 738, DateTimeKind.Utc).AddTicks(2324));

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAt",
                value: new DateTime(2026, 8, 20, 19, 26, 31, 738, DateTimeKind.Utc).AddTicks(5187));

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAt",
                value: new DateTime(2026, 8, 20, 19, 26, 31, 738, DateTimeKind.Utc).AddTicks(5203));

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "CreatedAt",
                value: new DateTime(2026, 8, 20, 19, 26, 31, 738, DateTimeKind.Utc).AddTicks(5207));

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                column: "CreatedAt",
                value: new DateTime(2026, 8, 20, 19, 26, 31, 738, DateTimeKind.Utc).AddTicks(5443));

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                column: "CreatedAt",
                value: new DateTime(2026, 8, 20, 19, 26, 31, 738, DateTimeKind.Utc).AddTicks(5678));

            migrationBuilder.InsertData(
                table: "Monsters",
                columns: new[] { "Id", "AttackPower", "Defense", "ExperienceReward", "Level", "LootItemId", "MaxHealth", "Name" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-000000000001"), 8, 2, 35, 1, new Guid("44444444-4444-4444-4444-444444444444"), 40, "Goblin Scout" },
                    { new Guid("20000000-0000-0000-0000-000000000002"), 15, 5, 80, 2, new Guid("11111111-1111-1111-1111-111111111111"), 75, "Skeleton Warrior" },
                    { new Guid("20000000-0000-0000-0000-000000000003"), 35, 12, 250, 4, new Guid("33333333-3333-3333-3333-333333333333"), 200, "Forest Drake" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Monsters_LootItemId",
                table: "Monsters",
                column: "LootItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Monsters");

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2026, 8, 20, 19, 9, 55, 937, DateTimeKind.Utc).AddTicks(4987));

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAt",
                value: new DateTime(2026, 8, 20, 19, 9, 55, 938, DateTimeKind.Utc).AddTicks(31));

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAt",
                value: new DateTime(2026, 8, 20, 19, 9, 55, 938, DateTimeKind.Utc).AddTicks(68));

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "CreatedAt",
                value: new DateTime(2026, 8, 20, 19, 9, 55, 938, DateTimeKind.Utc).AddTicks(73));

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                column: "CreatedAt",
                value: new DateTime(2026, 8, 20, 19, 9, 55, 938, DateTimeKind.Utc).AddTicks(538));

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                column: "CreatedAt",
                value: new DateTime(2026, 8, 20, 19, 9, 55, 938, DateTimeKind.Utc).AddTicks(963));
        }
    }
}
