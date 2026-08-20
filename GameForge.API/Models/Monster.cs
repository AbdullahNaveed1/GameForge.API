namespace GameForge.API.Models;

public class Monster
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Level { get; set; } = 1;
    public int MaxHealth { get; set; } = 50;
    public int AttackPower { get; set; } = 10;
    public int Defense { get; set; } = 2;
    public int ExperienceReward { get; set; } = 40;

    // Optional guaranteed or high-chance loot drop
    public Guid? LootItemId { get; set; }
    public Item? LootItem { get; set; }
}