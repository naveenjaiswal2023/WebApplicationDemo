using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            // 🔹 Simulate user authentication from DB
            if (model.Username == "admin" && model.Password == "password")
            {
                string role = "Admin"; // Set role dynamically based on user
                var token = GenerateJwtToken(model.Username, role);
                return Ok(new { Token = token });
            }
            else if (model.Username == "user" && model.Password == "password")
            {
                string role = "User"; // Regular user
                var token = GenerateJwtToken(model.Username, role);
                return Ok(new { Token = token });
            }

            return Unauthorized();
        }

        private string GenerateJwtToken(string username, string role)
        {
            var secretKey = _configuration["JwtSettings:SecretKey"]; // ✅ Get from appsettings
            var issuer = _configuration["JwtSettings:Issuer"]; // ✅ Get from appsettings
            var audience = _configuration["JwtSettings:Audience"]; // ✅ Get from appsettings

            if (string.IsNullOrWhiteSpace(secretKey) || secretKey.Length < 16)
            {
                throw new InvalidOperationException("JWT Secret Key must be at least 16 bytes long.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role), // 🔹 Role claim
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
