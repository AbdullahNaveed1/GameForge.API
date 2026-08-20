namespace GameForge.API.DTOs;

public record ItemResponseDto(
    Guid Id,
    string Name,
    string Description,
    string Type,
    string Rarity,
    int Value,
    int MaxStack,
    int AttackBonus,
    int DefenseBonus,
    int HealthRestore,
    int ManaRestore
);