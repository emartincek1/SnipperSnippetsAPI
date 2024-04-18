using Microsoft.EntityFrameworkCore;
using SnipperSnippetsAPI.Data;
using SnipperSnippetsAPI.Repositories;
using SnipperSnippetsAPI.Repositories.Contracts;
using SnipperSnippetsAPI.Utilities;

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
builder.Services.AddSingleton<EncryptUtility>();

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
