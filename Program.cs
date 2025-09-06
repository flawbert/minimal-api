var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "E aí Pessoal!");

app.MapPost("/login", (MinimalApi.DTOS.LoginDTO loginDTO) =>
{
    if (loginDTO.Email == "admin@teste.com" && loginDTO.Password == "123456")
        return Results.Ok("Login successful");
    else   
        return Results.Unauthorized();
});

app.Run();