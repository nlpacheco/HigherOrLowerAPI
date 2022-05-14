

//using Microsoft.Extensions.Configuration;

using HigherOrLowerData;
using HigherOrLowerData.Business;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// IT SHOULD USE SECRET CONFIGURATION
var connectionString =  builder.Configuration.GetConnectionString("higherlowerConnectionString");
//connectionString = connectionString.Replace("DATABASE_SERVER", Configuration.GetValue<string>("DATABASE_SERVER"));
//connectionString = connectionString.Replace("DATABASE_PORT", Configuration.GetValue("DATABASE_PORT", "3306"));
//connectionString = connectionString.Replace("DATABASE_PASSWORD", Configuration.GetValue("DATABASE_PASSWORD", "Urano44m"));

var serverVersion = new MySqlServerVersion(new Version(8, 0, 27));
builder.Services.AddDbContext<HigherOrLowerData.ApplicationDbContext>(
    options => options
                .UseMySql(connectionString, serverVersion, b => b.MigrationsAssembly("HigherOrLowerData"))
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
     );

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Game API - Higher or Lower",
        Description = "Backend Software Engineer Test - Norberto Luciano Pacheco",
    });
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWorkEF>();
//builder.Services.AddScoped<IRandomNumbers, RandomNumbers>();
builder.Services.AddScoped<IRandomNumbers, FakeRandomNumbers>();
builder.Services.AddScoped<GameLogic>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

