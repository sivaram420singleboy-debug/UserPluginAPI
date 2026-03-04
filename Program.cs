var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger
app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserPluginAPI v1");
    c.RoutePrefix = "";   // Swagger opens at root URL
    c.EnableDeepLinking();
});

app.MapControllers();

app.Run();