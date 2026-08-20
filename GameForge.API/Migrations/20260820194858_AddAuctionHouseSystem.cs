using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameForge.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAuctionHouseSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Gold",
                table: "Characters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AuctionListings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SellerCharacterId = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    PriceInGold = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsSold = table.Column<bool>(type: "boolean", nullable: false),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuctionListings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuctionListings_Characters_SellerCharacterId",
                        column: x => x.SellerCharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuctionListings_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2026, 8, 20, 19, 48, 57, 582, DateTimeKind.Utc).AddTicks(1031));

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAt",
                value: new DateTime(2026, 8, 20, 19, 48, 57, 582, DateTimeKind.Utc).AddTicks(3616));

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAt",
                value: new DateTime(2026, 8, 20, 19, 48, 57, 582, DateTimeKind.Utc).AddTicks(3630));

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "CreatedAt",
                value: new DateTime(2026, 8, 20, 19, 48, 57, 582, DateTimeKind.Utc).AddTicks(3634));

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                column: "CreatedAt",
                value: new DateTime(2026, 8, 20, 19, 48, 57, 582, DateTimeKind.Utc).AddTicks(3880));

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                column: "CreatedAt",
                value: new DateTime(2026, 8, 20, 19, 48, 57, 582, DateTimeKind.Utc).AddTicks(4130));

            migrationBuilder.CreateIndex(
                name: "IX_AuctionListings_ItemId",
                table: "AuctionListings",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionListings_SellerCharacterId",
                table: "AuctionListings",
                column: "SellerCharacterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuctionListings");

            migrationBuilder.DropColumn(
                name: "Gold",
                table: "Characters");

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
        }
    }
}
