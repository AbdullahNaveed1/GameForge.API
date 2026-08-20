using StackExchange.Redis;

namespace GameForge.API.Services;

public interface ILeaderboardService
{
    Task UpdateCharacterRankAsync(string characterName, double score);
    Task<List<LeaderboardEntryDto>> GetTopLeaderboardAsync(int topCount = 10);
}

public record LeaderboardEntryDto(int Rank, string CharacterName, double Score);

public class LeaderboardService : ILeaderboardService
{
    private readonly IConnectionMultiplexer _redis;
    private const string LeaderboardKey = "leaderboard:level_xp";

    public LeaderboardService(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task UpdateCharacterRankAsync(string characterName, double score)
    {
        try
        {
            var db = _redis.GetDatabase();
            await db.SortedSetAddAsync(LeaderboardKey, characterName, score);
        }
        catch
        {
            // Fail gracefully if local Redis instance is not active
        }
    }

    public async Task<List<LeaderboardEntryDto>> GetTopLeaderboardAsync(int topCount = 10)
    {
        var result = new List<LeaderboardEntryDto>();
        try
        {
            var db = _redis.GetDatabase();
            var entries = await db.SortedSetRangeByRankWithScoresAsync(LeaderboardKey, 0, topCount - 1, Order.Descending);

            int rank = 1;
            foreach (var entry in entries)
            {
                result.Add(new LeaderboardEntryDto(rank++, entry.Element.ToString(), entry.Score));
            }
        }
        catch
        {
            // Return empty list if Redis connection is unavailable
        }

        return result;
    }
}