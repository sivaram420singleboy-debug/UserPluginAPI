var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseStaticFiles();   // ⭐ THIS LINE MUST

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.InjectJavascript("/swagger-ui/custom.js");
});

app.UseAuthorization();

app.MapControllers();
app.MapGet("/", context =>
{
context.Response.Redirect("/index.html");
return Task.CompletedTask;
});
app.UseDefaultFiles();
app.UseStaticFiles();
app.Run();