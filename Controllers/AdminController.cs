using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Data.SQLite;

namespace UserPluginAPI.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize] // 🔐 full protection
    public class AdminController : ControllerBase
    {
        [HttpPost("generate")]
        public IActionResult Generate()
        {
            try
            {
                string key = "LIC-" + Guid.NewGuid().ToString("N")[..8].ToUpper();
                DateTime expiryDate = DateTime.Now.AddDays(30);

                using var con = new SQLiteConnection("Data Source=users.db");
                con.Open();

                string query = @"INSERT INTO Licenses 
                                 (LicenseKey, IsUsed, MachineId, ExpiryDate) 
                                 VALUES (@key, 0, NULL, @exp)";

                var cmd = new SQLiteCommand(query, con);
                cmd.Parameters.AddWithValue("@key", key);
                cmd.Parameters.AddWithValue("@exp", expiryDate.ToString("yyyy-MM-dd"));
                cmd.ExecuteNonQuery();

                return Ok(new
                {
                    message = "✅ License Generated",
                    license = key,
                    expiry = expiryDate.ToString("yyyy-MM-dd")
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "❌ " + ex.Message);
            }
        }
    }
}