using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "UserPluginAPI",
        Version = "v1"
    });
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.RoutePrefix = string.Empty;
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserPluginAPI v1");

    c.InjectJavascript("/swagger-custom.js");
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();