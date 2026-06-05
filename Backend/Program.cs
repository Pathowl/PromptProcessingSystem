using Backend.Database; 
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using Backend.Workers;
using MassTransit;
using System.Text.Json.Serialization;
using Backend.Services;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");

var connectionString = $"Host=localhost;Port=5432;Database=PromptDb;Username=admin;Password={dbPassword}";

// database connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers().AddJsonOptions(options =>
    {
        // allows sending strings instead of numbers for enums, for better readability in API responses
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }); 

// Add services to the container.
builder.Services.AddOpenApi();

// masstransit
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PromptWorker>();
    // rabbitmq usage
    x.UsingRabbitMq((context, cfg) =>
    {
        // rabbitmq connection settings
        cfg.Host("localhost", "/", h => {
            h.Username("guest");
            h.Password("guest");
        });
        cfg.ConcurrentMessageLimit = 1; // Only 1 message at a time for simplicity and checking statuses
        cfg.ConfigureEndpoints(context);
    });
});
builder.Services.AddHttpClient<GeminiService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // Database migration
    dbContext.Database.Migrate();
    //Deleting data every backend run for testing purposes, so we always start with an empty table and id=1 for easier checking of statuses
    try 
    {
        dbContext.Database.ExecuteSqlRaw("TRUNCATE TABLE \"Prompts\" RESTART IDENTITY CASCADE;");
        Console.WriteLine("[SYSTEM] Baza danych gotowa: Tabela 'Prompts' wyczyszczona.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[SYSTEM] Uwaga: Nie udało się wyczyścić tabeli (może jeszcze nie istnieje?): {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/weatherforecast", () => { /* ... */ });

app.UseCors("AllowFrontend"); // CORS
app.MapControllers(); //Controllery
app.Run();