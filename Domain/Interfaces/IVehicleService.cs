namespace MinimalApi.Domain.Interfaces;
using MinimalApi.Domain.Entities;

public interface IVehicleService
{
    List<Vehicle> GetAllVehicles(int? page = 1, string? name = null, string? brand = null);
    Vehicle? GetVehicleById(int id);
    void IncludeVehicle(Vehicle vehicle);
    void UpdateVehicle(Vehicle vehicle);
    void DeleteVehicle(Vehicle vehicle);

}