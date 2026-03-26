using Microsoft.AspNetCore.Http;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

public class AuthLicenseMiddleware
{
    private readonly RequestDelegate _next;

    public AuthLicenseMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower() ?? "";

        // 🔥 1. FULL BYPASS FOR LICENSE + PUBLIC
        if (path.StartsWith("/swagger") ||
            path.StartsWith("/api/login") ||
            path.StartsWith("/api/license") ||   // ✅ VERY IMPORTANT
            path.StartsWith("/index.html") ||
            path.StartsWith("/login.html") ||
            path.StartsWith("/admin.html") ||
            path == "/" ||
            path.Contains(".css") ||
            path.Contains(".js"))
        {
            await _next(context);
            return;
        }

        // 🔐 2. TOKEN CHECK
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("❌ Token missing");
            return;
        }

        // 🔐 3. LICENSE CHECK
        string machineId = MachineHelper.GetMachineId();

        // 🔥 FIX: USE SAME PATH AS CONTROLLER
        string dbPath = "/app/users.db";

        using (var con = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
        {
            con.Open();

            string query = "SELECT * FROM Licenses WHERE IsUsed=1 LIMIT 1";
            var cmd = new SQLiteCommand(query, con);

            var reader = cmd.ExecuteReader();

            if (!reader.HasRows)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("❌ License not activated");
                return;
            }

            reader.Read();

            string storedMachine = reader["MachineId"]?.ToString();
            string expiry = reader["ExpiryDate"]?.ToString();

            // 🔥 EXPIRY CHECK
            if (!string.IsNullOrEmpty(expiry))
            {
                if (DateTime.TryParse(expiry, out DateTime expDate))
                {
                    if (DateTime.Now > expDate)
                    {
                        context.Response.StatusCode = 403;
                        await context.Response.WriteAsync("❌ License expired");
                        return;
                    }
                }
            }

            // 🔥 MACHINE LOCK (STRICT)
            if (!string.IsNullOrEmpty(storedMachine) && storedMachine != machineId)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("❌ License already used on another PC");
                return;
            }
        }

        await _next(context);
    }
}