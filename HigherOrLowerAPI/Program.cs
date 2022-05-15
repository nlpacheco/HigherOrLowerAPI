

//using Microsoft.Extensions.Configuration;

using HigherOrLowerData;
using HigherOrLowerData.Business;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// IT SHOULD USE A SECRET CONFIGURATION
var connectionString =  builder.Configuration.GetConnectionString("higherlowerConnectionString");

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
        Description = "Backend Software Engineer Test",
        Contact = new OpenApiContact
        {
            Name = "Norberto Pacheco",
            Email = "norberto.luciano.pacheco@gmail.com"
        }
    });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

});


builder.Services.AddScoped<IUnitOfWork, UnitOfWorkEF>();
builder.Services.AddScoped<IRandomNumbers, RandomNumbers>();
//builder.Services.AddScoped<IRandomNumbers, FakeRandomNumbers>();
builder.Services.AddScoped<GameLogic>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsStaging())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

