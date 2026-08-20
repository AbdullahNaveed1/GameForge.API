using GameForge.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameForge.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeaderboardController : ControllerBase
{
    private readonly ILeaderboardService _leaderboardService;

    public LeaderboardController(ILeaderboardService leaderboardService)
    {
        _leaderboardService = leaderboardService;
    }

    // GET: api/leaderboard/top?count=10
    [HttpGet("top")]
    public async Task<ActionResult<List<LeaderboardEntryDto>>> GetTopRankings([FromQuery] int count = 10)
    {
        var rankings = await _leaderboardService.GetTopLeaderboardAsync(count);
        return Ok(rankings);
    }
}