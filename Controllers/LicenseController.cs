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
            public string LicenseKey { get; set; }
        }

        [HttpPost("activate")]
        [Produces("application/json")] // 🔥 Important
        public IActionResult Activate([FromBody] LicenseRequest req)
        {
            try
            {
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

                    if (!reader.HasRows)
                        return BadRequest(new { message = "❌ Invalid License" });

                    reader.Read();

                    string dbMachine = reader["MachineId"]?.ToString();
                    int isUsed = Convert.ToInt32(reader["IsUsed"]);

                    DateTime? expiryDate = null;
                    if (reader["ExpiryDate"] != DBNull.Value)
                        expiryDate = Convert.ToDateTime(reader["ExpiryDate"]);

                    // 🔥 EXPIRY CHECK
                    if (expiryDate.HasValue && expiryDate.Value < DateTime.Now)
                        return BadRequest(new { message = "❌ License expired" });

                    // 🔥 FIRST TIME ACTIVATION
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

                        return Ok(new { message = "License Activated" }); // 🔥 clean JSON
                    }

                    // 🔥 MACHINE LOCK
                    if (!string.IsNullOrEmpty(dbMachine) && dbMachine != machineId)
                        return BadRequest(new { message = "License already used on another PC" });

                    return Ok(new { message = "License Valid" });
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