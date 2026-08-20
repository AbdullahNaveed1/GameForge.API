using System.ComponentModel.DataAnnotations;

namespace GameForge.API.DTOs;

public record CreateCharacterDto(
    [Required, MinLength(3), MaxLength(32)] string Name,
    [Required] string CharacterClass
);

public record CharacterResponseDto(
    Guid Id,
    string Name,
    string CharacterClass,
    int Level,
    int Experience,
    int Health,
    int Mana,
    DateTime CreatedAt
);

// New DTOs for Day 4
public record AddExperienceDto(
    [Range(1, 100000, ErrorMessage = "Experience must be greater than 0.")] int Amount
);

public record UpdateStatsDto(
    [Range(0, 10000)] int Health,
    [Range(0, 10000)] int Mana
);