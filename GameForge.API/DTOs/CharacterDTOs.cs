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