//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.IdentityModel.Tokens;
//using RESTfulAPI.Services;
//using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//Redis 1
//builder.Services.AddSingleton<IConnectionMultiplexer>(redis =>
//    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection")));

//builder.Services.AddScoped<CacheService>();

//JWT
builder.Configuration.AddJsonFile("appsettings.json");
var secretKey = builder.Configuration.GetSection("Settings").GetSection("SecretKey").ToString();
var keyBytes = Encoding.UTF8.GetBytes(secretKey);

//builder.Services.AddAuthentication(config =>
//{

//    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

//}).AddJwtBearer(config =>
//{
//    config.RequireHttpsMetadata = false;
//    config.SaveToken = true;
//    config.TokenValidationParameters = new TokenValidationParameters
//    {

//        ValidateIssuerSigningKey = true,
//        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
//        ValidateIssuer = false,
//        ValidateAudience = false
//    };
//});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//Configure the HTTP request pipeline
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