using DLL1.Controllers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserPluginAPI v1");
    c.RoutePrefix = string.Empty; // Swagger at root
});

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();