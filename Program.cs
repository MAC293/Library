using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;
using Library.Services;
using Library.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

var builder = WebApplication.CreateBuilder(args);

//Custom Model Binder
builder.Services.AddControllers(options =>
{
    options.ModelBinderProviders.Insert(0, new CustomBinderProviderService());
});

//Context as Scoped
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Server=TUF293;Database=LibraryDB;Trusted_Connection=True;TrustServerCertificate=True;")));

//Redis connection as Singleton
builder.Services.AddSingleton<IConnectionMultiplexer>(redis =>
ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection")));

//CacheManagerService as Scoped
builder.Services.AddScoped<CacheManagerService>();

//CacheService as Scoped
builder.Services.AddScoped<CacheService>();

//ClaimVerifierService as Scoped
builder.Services.AddScoped<ClaimVerifierService>();

//Adding HttpContext allowing ClaimVerifierService to use User outside controller
builder.Services.AddHttpContextAccessor();

//Adding HelperService as Scoped to use a common method
builder.Services.AddScoped<HelperService>();

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

//Read from appsettings Serilog configuration
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration).CreateLogger();

var app = builder.Build();

//HTTP requests
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