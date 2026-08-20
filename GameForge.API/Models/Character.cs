using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameForge.API.Models;

public class Character
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(32)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string CharacterClass { get; set; } = "Warrior"; // e.g., Warrior, Mage, Rogue

    public int Level { get; set; } = 1;
    public int Experience { get; set; } = 0;
    public int Health { get; set; } = 100;
    public int Mana { get; set; } = 50;

    // Currency balance for Market / Auction House
    public int Gold { get; set; } = 100;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign Key linking to Player
    [Required]
    public Guid PlayerId { get; set; }

    // Navigation Property to Player
    [ForeignKey(nameof(PlayerId))]
    public Player Player { get; set; } = null!;

    // Navigation Property to Inventory
    public ICollection<InventoryItem> Inventory { get; set; } = new List<InventoryItem>();
}