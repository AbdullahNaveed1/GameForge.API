using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameForge.API.Models;

public class InventoryItem
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid CharacterId { get; set; }

    [ForeignKey(nameof(CharacterId))]
    public Character Character { get; set; } = null!;

    [Required]
    public Guid ItemId { get; set; }

    [ForeignKey(nameof(ItemId))]
    public Item Item { get; set; } = null!;

    [Range(1, 999)]
    public int Quantity { get; set; } = 1;

    public bool IsEquipped { get; set; } = false;

    public DateTime AcquiredAt { get; set; } = DateTime.UtcNow;
}