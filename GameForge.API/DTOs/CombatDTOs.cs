namespace GameForge.API.DTOs;

public record MonsterResponseDto(
    Guid Id,
    string Name,
    int Level,
    int MaxHealth,
    int AttackPower,
    int Defense,
    int ExperienceReward,
    string? PotentialDropName
);

public record BattleTurnLogDto(
    int Round,
    string Attacker,
    string Defender,
    int DamageDealt,
    int DefenderRemainingHealth
);

public record BattleResultDto(
    bool Victory,
    string Summary,
    int CharacterRemainingHealth,
    int GainedExperience,
    int CurrentLevel,
    string? DroppedItemName,
    List<BattleTurnLogDto> CombatLogs
);