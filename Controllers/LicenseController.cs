using Microsoft.AspNetCore.Mvc;
using System;
using System.Data.SQLite;

namespace UserPluginAPI.Controllers
{
    [ApiController]
    [Route("api/license")]
    public class LicenseController : ControllerBase
    {
        public class LicenseRequest
        {
            public string LicenseKey { get; set; } = "";
        }

        [HttpPost("activate")]
        [Produces("application/json")]
        public IActionResult Activate([FromBody] LicenseRequest req)
        {
            try
            {
                if (req == null || string.IsNullOrWhiteSpace(req.LicenseKey))
                    return BadRequest(new { message = "License key required" });

                string key = req.LicenseKey.Trim();
                string machineId = MachineHelper.GetMachineId();

                // ✅ Render safe DB path
                string dbPath = "/app/users.db";

                if (!System.IO.File.Exists(dbPath))
                    return StatusCode(500, new { message = "Database file not found" });

                using (var con = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                {
                    con.Open();

                    string query = "SELECT MachineId, IsUsed, ExpiryDate FROM Licenses WHERE LicenseKey=@key LIMIT 1";
                    using (var cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@key", key);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                                return BadRequest(new { message = "Invalid License" });

                            reader.Read();

                            string dbMachine = reader["MachineId"]?.ToString() ?? "";
                            int isUsed = reader["IsUsed"] != DBNull.Value ? Convert.ToInt32(reader["IsUsed"]) : 0;

                            DateTime? expiryDate = null;
                            if (reader["ExpiryDate"] != DBNull.Value)
                                expiryDate = Convert.ToDateTime(reader["ExpiryDate"]);

                            // 🔥 Expiry check
                            if (expiryDate.HasValue && expiryDate.Value < DateTime.Now)
                                return BadRequest(new { message = "License expired" });

                            // 🔥 First activation
                            if (isUsed == 0)
                            {
                                reader.Close();

                                string update = @"UPDATE Licenses 
                                                  SET IsUsed=1, MachineId=@machine 
                                                  WHERE LicenseKey=@key";

                                using (var updateCmd = new SQLiteCommand(update, con))
                                {
                                    updateCmd.Parameters.AddWithValue("@machine", machineId);
                                    updateCmd.Parameters.AddWithValue("@key", key);
                                    updateCmd.ExecuteNonQuery();
                                }

                                return Ok(new { message = "License Activated" });
                            }

                            // 🔥 Machine lock
                            if (!string.IsNullOrEmpty(dbMachine) && dbMachine != machineId)
                                return BadRequest(new { message = "License already used on another PC" });

                            return Ok(new { message = "License Valid" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Server Error",
                    error = ex.Message
                });
            }
        }
    }
}