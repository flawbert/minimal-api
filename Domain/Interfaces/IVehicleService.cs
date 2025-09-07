using MinimalApi.Domain.Entities;
using MinimalApi.DTOS;

namespace MinimalApi.Domain.Interfaces;

public interface IVehicleService
{
    List<Vehicle> GetAllVehicles(int page = 1, string? name = null, string? brand = null);
    Vehicle GetVehicleById(int id);
    Vehicle IncludeVehicle(Vehicle vehicle);
    Vehicle UpdateVehicle(Vehicle vehicle);
    bool DeleteVehicle(Vehicle vehicle);

}