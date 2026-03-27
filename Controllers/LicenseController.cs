using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Data.SQLite;

namespace UserPluginAPI.Controllers
{
    [ApiController]
    [Route("api/license")]
    [AllowAnonymous]
    public class LicenseController : ControllerBase
    {
        public class LicenseRequest
        {
            public string LicenseKey { get; set; } = "";
            public string MachineId { get; set; } = "";
        }

        [HttpPost("activate")]
        public IActionResult Activate([FromBody] LicenseRequest req)
        {
            try
            {
                // 🔥 VALIDATION
                if (req == null || string.IsNullOrWhiteSpace(req.LicenseKey))
                    return BadRequest(new { message = "License key required" });

                if (string.IsNullOrWhiteSpace(req.MachineId))
                    return BadRequest(new { message = "Machine ID missing" });

                string key = req.LicenseKey.Trim();
                string machineId = req.MachineId.Trim();

                string dbPath = "/app/users.db";

                using (var con = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                {
                    con.Open();

                    // 🔥 CREATE TABLE
                    var create = new SQLiteCommand(@"
                        CREATE TABLE IF NOT EXISTS Licenses (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            LicenseKey TEXT UNIQUE,
                            MachineId TEXT,
                            IsUsed INTEGER DEFAULT 0,
                            ExpiryDate TEXT
                        );
                    ", con);
                    create.ExecuteNonQuery();

                    // 🔥 INSERT DEFAULT LICENSE (ONLY FIRST TIME)
                    var check = new SQLiteCommand("SELECT COUNT(*) FROM Licenses", con);
                    long count = (long)check.ExecuteScalar();

                    if (count == 0)
                    {
                        var insert = new SQLiteCommand(@"
                            INSERT INTO Licenses (LicenseKey, MachineId, IsUsed, ExpiryDate)
                            VALUES ('ABC123-XYZ789','',0,NULL)
                        ", con);

                        insert.ExecuteNonQuery();
                    }

                    // 🔥 GET LICENSE
                    var cmd = new SQLiteCommand("SELECT * FROM Licenses WHERE LicenseKey=@key LIMIT 1", con);
                    cmd.Parameters.AddWithValue("@key", key);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return BadRequest(new { message = "❌ Invalid License" });

                        reader.Read();

                        string dbMachine = reader["MachineId"]?.ToString() ?? "";
                        int isUsed = reader["IsUsed"] != DBNull.Value ? Convert.ToInt32(reader["IsUsed"]) : 0;
                        string expiry = reader["ExpiryDate"]?.ToString();

                        // 🔥 EXPIRY CHECK (optional ready)
                        if (!string.IsNullOrEmpty(expiry))
                        {
                            DateTime expDate;
                            if (DateTime.TryParse(expiry, out expDate))
                            {
                                if (DateTime.UtcNow > expDate)
                                    return BadRequest(new { message = "❌ License Expired" });
                            }
                        }

                        // 🔥 FIRST TIME ACTIVATION
                        if (isUsed == 0)
                        {
                            reader.Close();

                            var update = new SQLiteCommand(@"
                                UPDATE Licenses 
                                SET IsUsed=1, MachineId=@machine 
                                WHERE LicenseKey=@key", con);

                            update.Parameters.AddWithValue("@machine", machineId);
                            update.Parameters.AddWithValue("@key", key);
                            update.ExecuteNonQuery();

                            return Ok(new { message = "✅ License Activated" });
                        }

                        // 🔥 MACHINE LOCK CHECK
                        if (!string.IsNullOrEmpty(dbMachine) && dbMachine != machineId)
                            return BadRequest(new { message = "❌ License already used on another PC" });

                        return Ok(new { message = "✅ License Valid" });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "❌ Server Error",
                    error = ex.Message
                });
            }
        }
    }
}