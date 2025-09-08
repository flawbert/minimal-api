using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interfaces;
using MinimalApi.Infrastructure.Db;

namespace MinimalApi.Domain.Services;

public class VehicleService : IVehicleService
{
    private readonly DbContexto _context;
    public VehicleService(DbContexto context)
    {
        _context = context;

    }

    public void DeleteVehicle(Vehicle vehicle)
    {
        _context.Vehicles.Remove(vehicle);
        _context.SaveChanges();
    }

    public List<Vehicle> GetAllVehicles(int? page = 1, string? name = null, string? brand = null)
    {
        var query = _context.Vehicles.AsQueryable();
        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(v => EF.Functions.Like(v.Name.ToLower(), $"%{name}%"));
        }

        int itensPerPage = 10;

        if (page != null)
        {
            query = query.Skip((int)(page - 1) * itensPerPage).Take(itensPerPage);
        }
 
        return query.ToList();
    }

    public Vehicle? GetVehicleById(int id)
    {
        return _context.Vehicles.Where(v => v.Id == id).FirstOrDefault();
    }

    public void IncludeVehicle(Vehicle vehicle)
    {
        _context.Vehicles.Add(vehicle);
        _context.SaveChanges();
    }

    public void UpdateVehicle(Vehicle vehicle)
    {
        _context.Vehicles.Update(vehicle);
        _context.SaveChanges();
    }
}