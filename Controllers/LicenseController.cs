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
        }

        [HttpPost("activate")]
        public IActionResult Activate([FromBody] LicenseRequest req)
        {
            try
            {
                if (req == null || string.IsNullOrWhiteSpace(req.LicenseKey))
                    return BadRequest(new { message = "License key required" });

                string key = req.LicenseKey.Trim();
                string machineId = MachineHelper.GetMachineId();

                string dbPath = "/app/users.db";

                using (var con = new SQLiteConnection($"Data Source={dbPath}"))
                {
                    con.Open();

                    // 🔥 Create table if not exists
                    var create = new SQLiteCommand(@"
                        CREATE TABLE IF NOT EXISTS Licenses (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            LicenseKey TEXT,
                            MachineId TEXT,
                            IsUsed INTEGER,
                            ExpiryDate TEXT
                        );
                    ", con);
                    create.ExecuteNonQuery();

                    // 🔥 Insert test license if empty
                    var check = new SQLiteCommand("SELECT COUNT(*) FROM Licenses", con);
                    long count = (long)check.ExecuteScalar();

                    if (count == 0)
                    {
                        var insert = new SQLiteCommand(@"
                            INSERT INTO Licenses (LicenseKey, MachineId, IsUsed, ExpiryDate)
                            VALUES ('ABC123','',0,NULL)
                        ", con);
                        insert.ExecuteNonQuery();
                    }

                    var cmd = new SQLiteCommand("SELECT * FROM Licenses WHERE LicenseKey=@key", con);
                    cmd.Parameters.AddWithValue("@key", key);

                    var reader = cmd.ExecuteReader();

                    if (!reader.HasRows)
                        return BadRequest(new { message = "Invalid License" });

                    reader.Read();

                    string dbMachine = reader["MachineId"]?.ToString() ?? "";
                    int isUsed = reader["IsUsed"] != DBNull.Value ? Convert.ToInt32(reader["IsUsed"]) : 0;

                    // 🔥 First activation
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

                        return Ok(new { message = "License Activated" });
                    }

                    // 🔥 Machine lock
                    if (!string.IsNullOrEmpty(dbMachine) && dbMachine != machineId)
                        return BadRequest(new { message = "Used on another machine" });

                    return Ok(new { message = "License Valid" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Server Error",
                    error = ex.ToString() // 🔥 FULL ERROR
                });
            }
        }
    }
}