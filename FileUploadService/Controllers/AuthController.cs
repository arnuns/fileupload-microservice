using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FileUploadService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace FileUploadService.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    public AuthController()
    {

    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] Login login)
    {
        if (login.Email == "arnun.s@outlook.com" && login.Password == "123456789")
        {
            var userId = 1; // Get userId from email and password
            var jwt = GenerateJwtToken(userId);
            var refreshToken = GenerateRefreshToken();

            // RefreshTokens.Add(new RefreshToken { Token = refreshToken, UserId = userId, ExpiryDate = DateTime.UtcNow.AddMinutes(60) });

            return Ok(new { token = jwt, refreshToken });
        }
        return Unauthorized();
    }

    // [HttpPost("refresh")]
    // public IActionResult Refresh([FromBody] string refreshToken)
    // {
    //     var storedToken = RefreshTokens.FirstOrDefault(t => t.Token == refreshToken);
    //     if (storedToken == null || storedToken.ExpiryDate < DateTime.UtcNow)
    //     {
    //         return Unauthorized();
    //     }

    //     var userId = storedToken.UserId;
    //     var user = new User { Id = userId };
    //     var jwt = GenerateJwtToken(user.Id);
    //     var newRefreshToken = GenerateRefreshToken();

    //     RefreshTokens.Remove(storedToken);
    //     RefreshTokens.Add(new RefreshToken { Token = newRefreshToken, UserId = userId, ExpiryDate = DateTime.UtcNow.AddMinutes(60) });

    //     return Ok(new { token = jwt, refreshToken = newRefreshToken });
    // }

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