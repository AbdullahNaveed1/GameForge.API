namespace GameForge.API.DTOs;

public record QuestResponseDto(
    Guid Id,
    string Title,
    string Description,
    int RequiredLevel,
    int ExperienceReward,
    string? RewardItemName
);

public record CompleteQuestResponseDto(
    string Message,
    int GainedExperience,
    int CurrentLevel,
    int CurrentExperience,
    string? GrantedItemName,
    int GrantedItemQuantity
);