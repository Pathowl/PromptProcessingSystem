using Backend.Database; 
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");

var connectionString = $"Host=localhost;Port=5432;Database=PromptDb;Username=admin;Password={dbPassword}";

// database connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add services to the container.
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/weatherforecast", () => { /* ... */ });

app.Run();