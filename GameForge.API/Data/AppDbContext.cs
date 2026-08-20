using GameForge.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace GameForge.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Player> Players => Set<Player>();
    public DbSet<Character> Characters => Set<Character>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<Quest> Quests => Set<Quest>();
    public DbSet<Monster> Monsters => Set<Monster>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Player configuration
        modelBuilder.Entity<Player>()
            .HasIndex(p => p.Username)
            .IsUnique();

        modelBuilder.Entity<Player>()
            .HasIndex(p => p.Email)
            .IsUnique();

        // Character - InventoryItem relationship
        modelBuilder.Entity<InventoryItem>()
            .HasOne(ii => ii.Character)
            .WithMany(c => c.Inventory)
            .HasForeignKey(ii => ii.CharacterId)
            .OnDelete(DeleteBehavior.Cascade);

        // Item - InventoryItem relationship
        modelBuilder.Entity<InventoryItem>()
            .HasOne(ii => ii.Item)
            .WithMany()
            .HasForeignKey(ii => ii.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        // Quest - Item relationship
        modelBuilder.Entity<Quest>()
            .HasOne(q => q.GuaranteedItemReward)
            .WithMany()
            .HasForeignKey(q => q.GuaranteedItemRewardId)
            .OnDelete(DeleteBehavior.SetNull);

        // Monster - Item Loot relationship
        modelBuilder.Entity<Monster>()
            .HasOne(m => m.LootItem)
            .WithMany()
            .HasForeignKey(m => m.LootItemId)
            .OnDelete(DeleteBehavior.SetNull);

        // Seed Items Catalog
        modelBuilder.Entity<Item>().HasData(
            new Item
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Rusty Sword",
                Description = "A weathered iron blade.",
                Type = ItemType.Weapon,
                Rarity = ItemRarity.Common,
                AttackBonus = 5,
                DefenseBonus = 0,
                MaxStack = 1
            },
            new Item
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Iron Plate Armor",
                Description = "Sturdy steel breastplate.",
                Type = ItemType.Armor,
                Rarity = ItemRarity.Uncommon,
                AttackBonus = 0,
                DefenseBonus = 12,
                MaxStack = 1
            },
            new Item
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Dragon Slayer Blade",
                Description = "Forged in ancient dragon fire.",
                Type = ItemType.Weapon,
                Rarity = ItemRarity.Legendary,
                AttackBonus = 150,
                DefenseBonus = 25,
                MaxStack = 1
            },
            new Item
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Name = "Lesser Health Potion",
                Description = "Restores 50 hit points.",
                Type = ItemType.Consumable,
                Rarity = ItemRarity.Common,
                HealthRestore = 50,
                MaxStack = 99
            },
            new Item
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Name = "Mana Flask",
                Description = "Restores 35 mana points.",
                Type = ItemType.Consumable,
                Rarity = ItemRarity.Common,
                ManaRestore = 35,
                MaxStack = 99
            },
            new Item
            {
                Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                Name = "Leather Boots",
                Description = "Light and agile footwear.",
                Type = ItemType.Armor,
                Rarity = ItemRarity.Common,
                AttackBonus = 0,
                DefenseBonus = 3,
                MaxStack = 1
            }
        );

        // Seed Deterministic Quests
        modelBuilder.Entity<Quest>().HasData(
            new Quest
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                Title = "Rats in the Cellar",
                Description = "Clear the cellar of vermin for the local tavernkeeper.",
                RequiredLevel = 1,
                ExperienceReward = 50,
                GuaranteedItemRewardId = Guid.Parse("44444444-4444-4444-4444-444444444444")
            },
            new Quest
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                Title = "The Goblin Outpost",
                Description = "Infiltrate and dismantle the vanguard camp near the forest edge.",
                RequiredLevel = 2,
                ExperienceReward = 150,
                GuaranteedItemRewardId = Guid.Parse("22222222-2222-2222-2222-222222222222")
            }
        );

        // Seed Monsters Catalog
        modelBuilder.Entity<Monster>().HasData(
            new Monster
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000001"),
                Name = "Goblin Scout",
                Level = 1,
                MaxHealth = 40,
                AttackPower = 8,
                Defense = 2,
                ExperienceReward = 35,
                LootItemId = Guid.Parse("44444444-4444-4444-4444-444444444444") // Drops Health Potion
            },
            new Monster
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000002"),
                Name = "Skeleton Warrior",
                Level = 2,
                MaxHealth = 75,
                AttackPower = 15,
                Defense = 5,
                ExperienceReward = 80,
                LootItemId = Guid.Parse("11111111-1111-1111-1111-111111111111") // Drops Rusty Sword
            },
            new Monster
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000003"),
                Name = "Forest Drake",
                Level = 4,
                MaxHealth = 200,
                AttackPower = 35,
                Defense = 12,
                ExperienceReward = 250,
                LootItemId = Guid.Parse("33333333-3333-3333-3333-333333333333") // Drops Dragon Slayer Blade
            }
        );
    }
}