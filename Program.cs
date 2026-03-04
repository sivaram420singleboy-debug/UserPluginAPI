var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.InjectJavascript("/swagger-ui/custom.js");
});

app.MapControllers();
app.UseAuthorization();

app.MapControllers();

// index.html → swagger redirect
app.MapGet("/index.html", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

app.Run();