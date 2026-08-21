using System.Threading.Tasks;
using GameForge.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameForge.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaderboardController : ControllerBase
    {
        private readonly ILeaderboardService _leaderboardService;

        public LeaderboardController(ILeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }

        [HttpGet("top")]
        public async Task<IActionResult> GetTop([FromQuery] int count = 10)
        {
            var top = await _leaderboardService.GetTopLeaderboardAsync(count);
            return Ok(top);
        }

        [Authorize]
        [HttpPost("submit-score")]
        public async Task<IActionResult> SubmitScore([FromQuery] string characterName, [FromQuery] double score)
        {
            await _leaderboardService.UpdateCharacterRankAsync(characterName, score);
            return Ok(new { message = "Score updated successfully." });
        }

        [HttpGet("rank/{characterName}")]
        public async Task<IActionResult> GetPlayerRankAndRivals(string characterName, [FromQuery] int radius = 2)
        {
            if (radius is < 1 or > 50)
            {
                radius = 2;
            }

            var result = await _leaderboardService.GetPlayerRadiusAsync(characterName, radius);
            if (result == null)
            {
                return NotFound(new { message = $"Character '{characterName}' has no recorded score." });
            }

            return Ok(result);
        }
    }
}