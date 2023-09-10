using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FileUploadService.Models;
using FileUploadService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace FileUploadService.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<AuthController> _logger;
    public AuthController(
        IUserService userService,
        ILogger<AuthController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    // // Only in Development
    // [HttpPost("[action]")]
    // public async Task<IActionResult> Create([FromBody] Login login)
    // {
    //     if (string.IsNullOrEmpty(login.Email) || string.IsNullOrEmpty(login.Password))
    //         return BadRequest();
    //     await _userService.CreateUserAsync(login.Email, login.Password);
    //     return Ok();
    // }

    [HttpPost("[action]")]
    public async Task<IActionResult> Login([FromBody] Login login)
    {
        if (string.IsNullOrEmpty(login.Email) || string.IsNullOrEmpty(login.Password))
            return Unauthorized();
        var user = await _userService.GetByEmailAsync(login.Email);
        if (user != null && BCrypt.Net.BCrypt.Verify(login.Password, user.Password))
        {
            var token = GenerateJwtToken(user.Id);
            var refreshToken = GenerateRefreshToken();
            await _userService.RefreshTokenAsync(user.Id, refreshToken, DateTime.UtcNow.AddMinutes(60));
            user.Token = token;
            user.RefreshToken = refreshToken;
            return Ok(user);
        }
        return Unauthorized();
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Refresh([FromBody] Refresh refresh)
    {
        var storedToken = await _userService.GetUserRefreshTokenAsync(refresh.RefreshToken!);
        if (storedToken == null || storedToken.ExpiryDate < DateTime.UtcNow)
            return Unauthorized();
        var userId = storedToken.UserId;
        var jwt = GenerateJwtToken(userId);
        var newRefreshToken = GenerateRefreshToken();
        var userRefreshToken = await _userService.RefreshTokenAsync(userId, newRefreshToken, DateTime.UtcNow.AddMinutes(60));
        return Ok(new { token = jwt, refreshToken = userRefreshToken.Token });
    }

    private string GenerateJwtToken(int userId)
    {
        var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
        if (string.IsNullOrEmpty(jwtSecret))
            throw new ArgumentNullException("JWT_SECRET environment variable is not set.");
        var key = Encoding.ASCII.GetBytes(jwtSecret);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", userId.ToString()) }),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}