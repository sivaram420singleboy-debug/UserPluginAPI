using DLL1.Controllers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.RoutePrefix = string.Empty;   // Swagger becomes home page
});

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();