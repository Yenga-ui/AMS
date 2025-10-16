using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AssetManagementSystem.DTO;
using AssetManagementSystem.Models;
using AssetManagementSystem.Services;

namespace AssetManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAdAuthenticationService _adService;
        private readonly AssetContext _context;
        private readonly IConfiguration _config;

        public AuthController(IAdAuthenticationService adService, AssetContext context, IConfiguration config)
        {
            _adService = adService;
            _context = context;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            var isAuthenticated = await _adService.AuthenticateAsync(login.Username, login.Password);

            if (!isAuthenticated)
            {
                return Unauthorized(new { message = "Invalid AD credentials" });
            }

            var appuser = await _context.DtUsers
                .FirstOrDefaultAsync(u => u.Email == login.Username);

            if (appuser == null)
            {
                return Unauthorized(new { message = "User not found in database." });
            }

            var token = GenerateJwtToken(appuser.Email, appuser.Email.Split('.')[0]); // Assuming appuser.Name exists

            return Ok(new
            {
                message = "Login successful",
                token = token,
                name = appuser.Email.Split('.')[0]
            });
        }

        private string GenerateJwtToken(string email, string name)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Email, email)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );
            try { 
            return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
