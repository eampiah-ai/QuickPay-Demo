using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IOptions<JwtConfig> config) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Login([FromBody] LoginDto dto)
    {
        var authenticated = string.Equals(dto.Username, dto.Password);

        if (!authenticated)
        {
            return Unauthorized();
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(config.Value.Key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new(ClaimTypes.Name, dto.Username)
            ]),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = config.Value.Issuer,
            Audience = config.Value.Audience
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Ok(new { Token = tokenHandler.WriteToken(token) });
    }

    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}