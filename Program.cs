using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
var pluginPath = Path.Combine(Directory.GetCurrentDirectory(), "plugins");

if (Directory.Exists(pluginPath))
{
    foreach (var dll in Directory.GetFiles(pluginPath, "*.dll"))
    {
        var assembly = Assembly.LoadFrom(dll);

        builder.Services
            .AddControllers()
            .PartManager
            .ApplicationParts
            .Add(new AssemblyPart(assembly));
    }
}

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();