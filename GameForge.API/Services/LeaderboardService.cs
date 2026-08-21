using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GameForge.API.Services
{
    public class LeaderboardEntryDto
    {
        [JsonPropertyName("characterName")]
        public string CharacterName { get; set; } = string.Empty;

        [JsonPropertyName("score")]
        public double Score { get; set; }

        [JsonPropertyName("rank")]
        public long Rank { get; set; }
    }

    public class LeaderboardRadiusResponseDto
    {
        [JsonPropertyName("targetCharacterName")]
        public string TargetCharacterName { get; set; } = string.Empty;

        [JsonPropertyName("targetRank")]
        public long TargetRank { get; set; }

        [JsonPropertyName("targetScore")]
        public double TargetScore { get; set; }

        [JsonPropertyName("surroundingRivals")]
        public List<LeaderboardEntryDto> SurroundingRivals { get; set; } = new();
    }

    public interface ILeaderboardService
    {
        Task<bool> UpdateCharacterRankAsync(string characterName, double score);
        Task<List<LeaderboardEntryDto>> GetTopLeaderboardAsync(int count = 10);
        Task<LeaderboardRadiusResponseDto?> GetPlayerRadiusAsync(string characterName, int radius = 2);
    }

    public class LeaderboardService : ILeaderboardService
    {
        private readonly IConnectionMultiplexer _redis;
        private const string LeaderboardKey = "leaderboard:global";

        public LeaderboardService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task<bool> UpdateCharacterRankAsync(string characterName, double score)
        {
            var db = _redis.GetDatabase();
            return await db.SortedSetAddAsync(LeaderboardKey, characterName, score);
        }

        public async Task<List<LeaderboardEntryDto>> GetTopLeaderboardAsync(int count = 10)
        {
            var db = _redis.GetDatabase();
            var entries = await db.SortedSetRangeByRankWithScoresAsync(
                LeaderboardKey,
                0,
                count - 1,
                Order.Descending
            );

            var result = new List<LeaderboardEntryDto>();
            long rank = 1;

            foreach (var entry in entries)
            {
                result.Add(new LeaderboardEntryDto
                {
                    CharacterName = entry.Element.ToString(),
                    Score = entry.Score,
                    Rank = rank++
                });
            }

            return result;
        }

        public async Task<LeaderboardRadiusResponseDto?> GetPlayerRadiusAsync(string characterName, int radius = 2)
        {
            var db = _redis.GetDatabase();

            //  Correct
            var rank0 = await db.SortedSetRankAsync(LeaderboardKey, characterName, Order.Descending);
            if (!rank0.HasValue)
            {
                return null;
            }

            var score = await db.SortedSetScoreAsync(LeaderboardKey, characterName);
            long currentRank = rank0.Value;

            long start = Math.Max(0, currentRank - radius);
            long stop = currentRank + radius;

            var entries = await db.SortedSetRangeByRankWithScoresAsync(
                LeaderboardKey,
                start,
                stop,
                Order.Descending
            );

            var rivals = new List<LeaderboardEntryDto>();
            long loopRank = start + 1;

            foreach (var entry in entries)
            {
                rivals.Add(new LeaderboardEntryDto
                {
                    CharacterName = entry.Element.ToString(),
                    Score = entry.Score,
                    Rank = loopRank++
                });
            }

            return new LeaderboardRadiusResponseDto
            {
                TargetCharacterName = characterName,
                TargetRank = currentRank + 1,
                TargetScore = score ?? 0,
                SurroundingRivals = rivals
            };
        }
    }
}