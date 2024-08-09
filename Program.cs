using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;
using Library.Services;
using Library.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//Redis connection as Singleton
builder.Services.AddSingleton<IConnectionMultiplexer>(redis =>
ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection")));

//CacheService as Scoped
builder.Services.AddScoped<CacheService>();

//Context as Scoped
builder.Services.AddDbContext<LibraryDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("Server=TUF293;Database=LibraryDB;Trusted_Connection=True;TrustServerCertificate=True;")));

//JWT
builder.Configuration.AddJsonFile("appsettings.json");
var secretKey = builder.Configuration.GetSection("Settings").GetSection("SecretKey").ToString();
var keyBytes = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(config =>
{

    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(config =>
{
    config.RequireHttpsMetadata = false;
    config.SaveToken = true;
    config.TokenValidationParameters = new TokenValidationParameters
    {

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//Configure the HTTP requests
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();