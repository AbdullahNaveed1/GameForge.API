using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GameForge.API.Data;
using GameForge.API.DTOs;
using GameForge.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace GameForge.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto dto)
    {
        if (await _context.Players.AnyAsync(p => p.Email == dto.Email))
        {
            return BadRequest("Email is already registered.");
        }

        if (await _context.Players.AnyAsync(p => p.Username == dto.Username))
        {
            return BadRequest("Username is already taken.");
        }

        var player = new Player
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Level = 1,
            Experience = 0,
            CreatedAt = DateTime.UtcNow
        };

        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        var token = GenerateJwtToken(player);

        return Ok(new AuthResponseDto
        {
            Token = token,
            Player = new PlayerResponseDto
            {
                Id = player.Id,
                Username = player.Username,
                Email = player.Email,
                Level = player.Level,
                Experience = player.Experience,
                CreatedAt = player.CreatedAt
            }
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto dto)
    {
        var player = await _context.Players.FirstOrDefaultAsync(p => p.Email == dto.Email);
        if (player == null || !BCrypt.Net.BCrypt.Verify(dto.Password, player.PasswordHash))
        {
            return Unauthorized("Invalid email or password.");
        }

        var token = GenerateJwtToken(player);

        return Ok(new AuthResponseDto
        {
            Token = token,
            Player = new PlayerResponseDto
            {
                Id = player.Id,
                Username = player.Username,
                Email = player.Email,
                Level = player.Level,
                Experience = player.Experience,
                CreatedAt = player.CreatedAt
            }
        });
    }

    private string GenerateJwtToken(Player player)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, player.Id.ToString()),
            new Claim(ClaimTypes.Name, player.Username),
            new Claim(ClaimTypes.Email, player.Email)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}