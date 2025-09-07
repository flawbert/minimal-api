using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interfaces;
using MinimalApi.DTOS;
using MinimalApi.Infrastructure.Db;

namespace MinimalApi.Domain.Services;

public class VehicleService : IVehicleService
{
    private readonly DbContexto _context;
    public VehicleService(DbContexto context)
    {
        _context = context;

    }

    public bool DeleteVehicle(Vehicle vehicle)
    {
        throw new NotImplementedException();
    }

    public List<Vehicle> GetAllVehicles(int page = 1, string? name = null, string? brand = null)
    {
        throw new NotImplementedException();
    }

    public Vehicle GetVehicleById(int id)
    {
        throw new NotImplementedException();
    }

    public Vehicle IncludeVehicle(Vehicle vehicle)
    {
        throw new NotImplementedException();
    }

    public Vehicle UpdateVehicle(Vehicle vehicle)
    {
        throw new NotImplementedException();
    }
}