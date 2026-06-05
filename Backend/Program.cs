using Backend.Database; 
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");

var connectionString = $"Host=localhost;Port=5432;Database=PromptDb;Username=admin;Password={dbPassword}";

// database connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add services to the container.
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // Database migration
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/weatherforecast", () => { /* ... */ });

app.UseCors("AllowFrontend"); // CORS

app.Run();