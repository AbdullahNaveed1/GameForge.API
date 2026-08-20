using System.ComponentModel.DataAnnotations;

namespace GameForge.API.Models;

public enum ItemType
{
    Weapon,
    Armor,
    Consumable,
    Material,
    Quest
}

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

public class Item
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(64)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(256)]
    public string Description { get; set; } = string.Empty;

    public ItemType Type { get; set; } = ItemType.Consumable;

    public ItemRarity Rarity { get; set; } = ItemRarity.Common;

    public int Value { get; set; } = 0;

    public int MaxStack { get; set; } = 1;

    public int AttackBonus { get; set; } = 0;
    public int DefenseBonus { get; set; } = 0;
    public int HealthRestore { get; set; } = 0;
    public int ManaRestore { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Add this navigation property inside the class:
    public ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
}