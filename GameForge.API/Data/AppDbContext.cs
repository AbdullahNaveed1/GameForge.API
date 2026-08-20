using GameForge.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GameForge.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Player> Players => Set<Player>();
    public DbSet<Character> Characters => Set<Character>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>(); // Register DbSet

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Player -> Character (1-to-Many)
        modelBuilder.Entity<Character>()
            .HasOne(c => c.Player)
            .WithMany(p => p.Characters)
            .HasForeignKey(c => c.PlayerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Character -> InventoryItem (1-to-Many)
        modelBuilder.Entity<InventoryItem>()
            .HasOne(ii => ii.Character)
            .WithMany(c => c.Inventory)
            .HasForeignKey(ii => ii.CharacterId)
            .OnDelete(DeleteBehavior.Cascade);

        // Item -> InventoryItem (1-to-Many)
        modelBuilder.Entity<InventoryItem>()
            .HasOne(ii => ii.Item)
            .WithMany(i => i.InventoryItems)
            .HasForeignKey(ii => ii.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        // Static seed timestamp
        var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<Item>().HasData(
            new Item
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Iron Longsword",
                Description = "A sturdy standard-issue iron longsword.",
                Type = ItemType.Weapon,
                Rarity = ItemRarity.Common,
                Value = 50,
                MaxStack = 1,
                AttackBonus = 15,
                CreatedAt = seedDate
            },
            new Item
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Apprentice Wand",
                Description = "A wooden wand that channels basic arcane power.",
                Type = ItemType.Weapon,
                Rarity = ItemRarity.Common,
                Value = 45,
                MaxStack = 1,
                AttackBonus = 12,
                CreatedAt = seedDate
            },
            new Item
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Steel Plate Armor",
                Description = "Heavy armor forged to withstand crushing blows.",
                Type = ItemType.Armor,
                Rarity = ItemRarity.Uncommon,
                Value = 120,
                MaxStack = 1,
                DefenseBonus = 25,
                CreatedAt = seedDate
            },
            new Item
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Name = "Lesser Health Potion",
                Description = "Instantly restores 50 Health points.",
                Type = ItemType.Consumable,
                Rarity = ItemRarity.Common,
                Value = 15,
                MaxStack = 99,
                HealthRestore = 50,
                CreatedAt = seedDate
            },
            new Item
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Name = "Lesser Mana Potion",
                Description = "Instantly restores 30 Mana points.",
                Type = ItemType.Consumable,
                Rarity = ItemRarity.Common,
                Value = 15,
                MaxStack = 99,
                ManaRestore = 30,
                CreatedAt = seedDate
            },
            new Item
            {
                Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                Name = "Dragon Slayer Blade",
                Description = "A legendary greatsword imbued with primordial flame.",
                Type = ItemType.Weapon,
                Rarity = ItemRarity.Legendary,
                Value = 2500,
                MaxStack = 1,
                AttackBonus = 150,
                CreatedAt = seedDate
            }
        );
    }
}