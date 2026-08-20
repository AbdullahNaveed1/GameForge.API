namespace GameForge.API.Models;

public class Quest
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int RequiredLevel { get; set; } = 1;
    public int ExperienceReward { get; set; }
    public Guid? GuaranteedItemRewardId { get; set; }
    public Item? GuaranteedItemReward { get; set; }
}