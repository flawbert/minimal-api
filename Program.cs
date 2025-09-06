using Microsoft.EntityFrameworkCore;
using MinimalApi.Infrastructure.Db;
using MinimalApi.DTOS;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DbContexto>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    );
});

var app = builder.Build();

app.MapGet("/", () => "E aí Pessoal!");

app.MapPost("/login", (LoginDTO loginDTO) =>
{
    if (loginDTO.Email == "admin@teste.com" && loginDTO.Password == "123456")
        return Results.Ok("Login successful");
    else   
        return Results.Unauthorized();
});

app.Run();