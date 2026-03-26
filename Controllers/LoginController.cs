using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Data.SQLite;

namespace UserPluginAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly string secretKey = "THIS_IS_SUPER_SECRET_KEY_12345"; // 🔥 SAME KEY

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // ❌ NULL CHECK
            if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("❌ Username or Password missing");
            }

            // ❌ WRONG LOGIN
            if (request.Username != "admin" || request.Password != "1234")
            {
                return Unauthorized("❌ Invalid username or password");
            }

            string machineId = MachineHelper.GetMachineId();

            using (SQLiteConnection con = new SQLiteConnection("Data Source=users.db"))
            {
                con.Open();

                string query = "SELECT * FROM Licenses WHERE IsUsed=1 LIMIT 1";
                SQLiteCommand cmd = new SQLiteCommand(query, con);

                var reader = cmd.ExecuteReader();

                if (!reader.HasRows)
                {
                    return BadRequest("❌ Please activate license first");
                }

                reader.Read();

                string storedMachine = reader["MachineId"]?.ToString();

                if (!string.IsNullOrEmpty(storedMachine) && storedMachine != machineId)
                {
                    return BadRequest("❌ License already used on another PC");
                }
            }

            // 🔐 TOKEN GENERATE
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, request.Username),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return Ok(new
            {
                message = "✅ Login Success",
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}