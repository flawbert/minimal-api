using Microsoft.EntityFrameworkCore;
using MinimalApi.Infrastructure.Db;
using MinimalApi.Domain.Services;
using MinimalApi.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Domain.ModelViews;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Enuns;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;

#region Builder
var builder = WebApplication.CreateBuilder(args);

var key = builder.Configuration.GetSection("Jwt").ToString();
if (string.IsNullOrEmpty(key)) key = "123456";

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "Jwt",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token in the text"
    });
    
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});



builder.Services.AddDbContext<DbContexto>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    );
});

var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
#endregion

#region Admins

string generateJwtToken(Admin admin)
{
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>()
    {
        new("Email", admin.Email),
        new("Profile", admin.Profile),
        new(ClaimTypes.Role, admin.Profile)
    };
    var token = new JwtSecurityToken(
       claims: claims,
       expires: DateTime.Now.AddDays(1),
       signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdminService adminService) =>
{
    var admin = adminService.Login(loginDTO);
    if (admin != null)
    {
        var token = generateJwtToken(admin);
        return Results.Ok(new LoggedAdmin
        {
            Email = admin.Email,
            Profile = admin.Profile, //(Profile)Enum.Parse(typeof(Profile), admin.Profile),
            Token = token
        });
    }
    else
        return Results.Unauthorized();
}).AllowAnonymous().WithTags("Admins");

app.MapPost("/admins", ([FromBody] AdminDTO adminDTO, IAdminService adminService) =>
{
    var validation = new ValidationErrors();
    validation.Messages = new List<string>();
    if (string.IsNullOrEmpty(adminDTO.Email))
        validation.Messages.Add("The field Email is required.");

    if (string.IsNullOrEmpty(adminDTO.Password))
        validation.Messages.Add("The field Password is required.");

    if (adminDTO.Profile == null)
        validation.Messages.Add("The field Profile is required.");

    if (validation.Messages.Count > 0)
        return Results.BadRequest(validation);

    var admin = new Admin
    {
        Email = adminDTO.Email,
        Password = adminDTO.Password,
        Profile = adminDTO.Profile.ToString() ?? Profile.Editor.ToString()//(Profile)Enum.Parse(typeof(Profile), adminDTO.Profile)
    };
    adminService.IncludeAdmin(admin);
    return Results.Created($"/admins/{admin.Id}", admin);
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" })
.WithTags("Admins");

app.MapGet("/admins", ([FromQuery] int? page, IAdminService adminService) =>
{
    var adms = new List<AdminModelView>();
    var admins = adminService.GetAllAdmins(page);
    foreach (var admin in admins)
    {
        adms.Add(new AdminModelView
        {
            Id = admin.Id,
            Email = admin.Email,
            Profile = admin.Profile //(Profile)Enum.Parse(typeof(Profile), admin.Profile)
        });
    }
    return Results.Ok(admins);
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" })
.WithTags("Admins");

app.MapGet("/admins/{id}", (int id, IAdminService adminService) =>
{
    var admin = adminService.GetAdminById(id);
    if (admin == null)
        return Results.NotFound();

    return Results.Ok(admin);
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" })
.WithTags("Admins");

#endregion

#region Vehicles
ValidationErrors validationDTO(VehicleDTO vehicleDTO)
{
    var validation = new ValidationErrors();
    validation.Messages = new List<string>();

    if (string.IsNullOrEmpty(vehicleDTO.Name))
        validation.Messages.Add("The field Name is required.");

    if (string.IsNullOrEmpty(vehicleDTO.Brand))
        validation.Messages.Add("The field Brand is required.");

    if (vehicleDTO.Year <= 1950)
        validation.Messages.Add("The vehicle is too old. Year must be greater than 1950.");

    return validation;
}

app.MapPost("/vehicles", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
{
    var validation = validationDTO(vehicleDTO);
    if (validation.Messages.Count > 0)
        return Results.BadRequest(validation);

    var vehicle = new Vehicle
    {
        Name = vehicleDTO.Name,
        Brand = vehicleDTO.Brand,
        Year = vehicleDTO.Year
    };
    vehicleService.IncludeVehicle(vehicle);

    return Results.Created($"/vehicles/{vehicle.Id}", vehicle);
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Admin, Editor" })
.WithTags("Vehicles");

app.MapGet("/vehicles", ([FromQuery] int? page, IVehicleService vehicleService) =>
{
    var vehicles = vehicleService.GetAllVehicles(page);
    return Results.Ok(vehicles);
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Admin, Editor" })
.WithTags("Vehicles");

app.MapGet("/vehicles/{id}", (int id, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.GetVehicleById(id);
    if (vehicle == null)
        return Results.NotFound();

    return Results.Ok(vehicle);
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Admin, Editor" })
.WithTags("Vehicles");

app.MapPut("/vehicles/{id}", ([FromRoute] int id, VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.GetVehicleById(id);
    if (vehicle == null)
        return Results.NotFound();

    var validation = validationDTO(vehicleDTO);
    if (validation.Messages.Count > 0)
        return Results.BadRequest(validation);

    vehicle.Name = vehicleDTO.Name;
    vehicle.Brand = vehicleDTO.Brand;
    vehicle.Year = vehicleDTO.Year;

    vehicleService.UpdateVehicle(vehicle);
    return Results.Ok(vehicle);
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" })
.WithTags("Vehicles");

app.MapDelete("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.GetVehicleById(id);
    if (vehicle == null)
        return Results.NotFound();

    vehicleService.DeleteVehicle(vehicle);
    return Results.NoContent();
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" })
.WithTags("Vehicles");

#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
#endregion