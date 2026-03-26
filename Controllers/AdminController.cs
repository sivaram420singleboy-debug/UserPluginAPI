using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;   // 🔥 ADD THIS
using System.Data.SQLite;



namespace UserPluginAPI.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize]   // 🔐 FULL CONTROLLER PROTECTION
    public class AdminController : ControllerBase
    {
       [Authorize]
[HttpPost("generate")]
        public IActionResult Generate()
        {
            try
            {
                // 🔑 Generate License Key
                string key = "LIC-" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();

                // ⏳ Expiry (30 days)
                DateTime expiryDate = DateTime.Now.AddDays(30);

                using (var con = new SQLiteConnection("Data Source=users.db"))
                {
                    con.Open();

                    string query = @"INSERT INTO Licenses 
                                     (LicenseKey, IsUsed, MachineId, ExpiryDate) 
                                     VALUES (@key, 0, NULL, @exp)";

                    var cmd = new SQLiteCommand(query, con);

                    cmd.Parameters.AddWithValue("@key", key);
                    cmd.Parameters.AddWithValue("@exp", expiryDate.ToString("yyyy-MM-dd"));

                    cmd.ExecuteNonQuery();
                }

                return Ok(new
                {
                    message = "✅ License Generated",
                    license = key,
                    expiry = expiryDate.ToString("yyyy-MM-dd")
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "❌ Error: " + ex.Message);
            }
        }
    }
}