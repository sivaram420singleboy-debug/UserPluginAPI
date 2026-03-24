using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;   // 🔥 Add this
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace UserPluginAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly string secretKey = "ThisIsASecretKeyForJWTAuthentication123!";

        // 🔥 Login API open ஆக இருக்கணும்
      [AllowAnonymous]
[HttpPost("login")]
public IActionResult Login(string username, string password)
        {
            // ✅ Username & Password Validation
            if (username != "admin" || password != "1234")
            {
                return Unauthorized("Invalid username or password");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return Ok(new
            {
                message = "Login Success",
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
    }
}