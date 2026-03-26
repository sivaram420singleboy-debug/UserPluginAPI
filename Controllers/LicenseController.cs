using Microsoft.AspNetCore.Mvc;
using System.Data.SQLite;
using Microsoft.AspNetCore.Authorization;


namespace UserPluginAPI.Controllers
{
    [ApiController]
    [Route("api/license")]
    public class LicenseController : ControllerBase
    {
        public class LicenseRequest
        {
            public string LicenseKey { get; set; }
        }

        [HttpPost("activate")]
        public IActionResult Activate([FromBody] LicenseRequest req)
        {
            // 🔥 Input validation
            if (req == null || string.IsNullOrEmpty(req.LicenseKey))
                return BadRequest(new { message = "❌ License key required" });

            string key = req.LicenseKey;
            string machineId = MachineHelper.GetMachineId();

            using (var con = new SQLiteConnection("Data Source=users.db"))
            {
                con.Open();

                string query = "SELECT * FROM Licenses WHERE LicenseKey=@key LIMIT 1";
                var cmd = new SQLiteCommand(query, con);
                cmd.Parameters.AddWithValue("@key", key);

                var reader = cmd.ExecuteReader();

                // 🔥 Invalid license
              if (!reader.HasRows)
    return BadRequest(new { message = "❌ Invalid License" });

if (expiryDate.HasValue && expiryDate.Value < DateTime.Now)
    return BadRequest(new { message = "❌ License expired" });

if (req == null || string.IsNullOrEmpty(req.LicenseKey))
    return BadRequest(new { message = "❌ License key required" });

if (!string.IsNullOrEmpty(dbMachine) && dbMachine != machineId)
    return BadRequest(new { message = "❌ License already used on another PC" });

                // 🔥 First activation
                if (isUsed == 0)
                {
                    reader.Close();

                    string update = @"UPDATE Licenses 
                                      SET IsUsed=1, MachineId=@machine 
                                      WHERE LicenseKey=@key";

                    var updateCmd = new SQLiteCommand(update, con);
                    updateCmd.Parameters.AddWithValue("@machine", machineId);
                    updateCmd.Parameters.AddWithValue("@key", key);
                    updateCmd.ExecuteNonQuery();

                    return Ok(new { message = "✅ License Activated" });
                }

                // 🔥 Machine lock (important)
                if (!string.IsNullOrEmpty(dbMachine) && dbMachine != machineId)
                    return BadRequest(new { message = "❌ License already used on another PC" });

                // 🔥 Already valid
                return Ok(new { message = "✅ License Valid" });
            }
        }
    }
}