using System.ComponentModel.DataAnnotations;

namespace GameForge.API.DTOs;

public record AddInventoryItemDto(
    [Required] Guid ItemId,
    [Range(1, 99)] int Quantity = 1
);

public record InventoryItemResponseDto(
    Guid InventoryItemId,
    Guid ItemId,
    string Name,
    string Description,
    string Type,
    string Rarity,
    int Quantity,
    bool IsEquipped,
    int AttackBonus,
    int DefenseBonus,
    int HealthRestore,
    int ManaRestore
);