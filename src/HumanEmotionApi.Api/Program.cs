using System.Text.Json.Serialization;
using HumanEmotionApi.Api.Middleware;
using HumanEmotionApi.Application;
using HumanEmotionApi.Infrastructure;
using HumanEmotionApi.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Human Emotion API",
        Version = "v1",
        Description = ".NET 10 LTS CRUD API for Humans with Emotion endpoints, CQRS, Repository and Unit of Work patterns, and dynamic ShowColor spectrum."
    });
});

// Clean Architecture Layer Registrations
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Human Emotion API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Ensure SQLite database schema is initialized
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<HumanEmotionDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

app.Run();

// Required for WebApplicationFactory in integration tests
public partial class Program { }

