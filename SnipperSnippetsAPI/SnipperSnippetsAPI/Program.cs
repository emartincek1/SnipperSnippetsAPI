using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SnipperSnippetsAPI.Data;
using SnipperSnippetsAPI.Middleware;
using SnipperSnippetsAPI.Models;
using SnipperSnippetsAPI.Repositories;
using SnipperSnippetsAPI.Repositories.Contracts;
using SnipperSnippetsAPI.Services;
using SnipperSnippetsAPI.Utilities;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContextPool<SnipperSnippetsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SnipperSnippetsConnection"))
);

builder.Services.AddScoped<ISnipperSnippetRepository, SnipperSnippetRepository>();
builder.Services.AddScoped<IIdentityServiceRepository, IdentityServiceRepository>();
builder.Services.AddSingleton<EncryptUtility>();
builder.Services.AddSingleton<IIdentityService, IdentityService>();



// configure JWT settings
var jwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettingsSection);
var jwtSettings = jwtSettingsSection.Get<JwtSettings>();
if (jwtSettings is null || jwtSettings.Secret is null)
{
    throw new ApplicationException("JwtSettings.Secret is null. Ensure it is set in configuration.");
}
var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<BasicAuthMiddleware>();
app.UseMiddleware<JwtMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
