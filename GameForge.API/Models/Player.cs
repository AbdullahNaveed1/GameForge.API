namespace GameForge.API.Models;

public class Player
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Level { get; set; } = 1;
    public int Experience { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}