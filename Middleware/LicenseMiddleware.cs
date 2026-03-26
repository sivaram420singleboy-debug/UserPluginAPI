using Microsoft.AspNetCore.Http;
using System.Data.SQLite;
using System.Threading.Tasks;

public class LicenseMiddleware
{
    private readonly RequestDelegate _next;

    public LicenseMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        // Allow license activation மட்டும்
        if (context.Request.Path.StartsWithSegments("/api/license"))
        {
            await _next(context);
            return;
        }

        bool isValid = false;

        using (SQLiteConnection con = new SQLiteConnection("Data Source=users.db"))
        {
            con.Open();

            string query = "SELECT * FROM Licenses WHERE IsUsed=1";
            SQLiteCommand cmd = new SQLiteCommand(query, con);

            var reader = cmd.ExecuteReader();

            if (reader.HasRows)
                isValid = true;
        }

        if (!isValid)
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("❌ License Not Activated");
            return;
        }

        await _next(context);
    }
}